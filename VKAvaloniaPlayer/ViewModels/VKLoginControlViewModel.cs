using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Utils;
using Button = Avalonia.Controls.Button;
using System.Windows;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Avalonia.Controls;
using System.Net.Sockets;

namespace VKAvaloniaPlayer.ViewModels
{
    public class VkLoginControlViewModel : ViewModelBase
    {
        private const string AuthUrl =
           @"https://oauth.vk.com/oauth/authorize?client_id=6121396" +
           "&scope=1073737727" +
           "&redirect_uri=https://oauth.vk.com/blank.html" +
           "&display=mobile" +
           "&response_type=token" +
           "&revoke=1";

        private Process _BrowserProcess;


        private  WebElement.WebElementServer _webElementServer;
        private int _ActiveAccountSelectIndex = -1;
        private string _InfoText = string.Empty;
        

        private bool _SavedAccountsIsVisible;

        public VkLoginControlViewModel()
        {
            
            LoadSavedAccounts();
            ToggleAccountsSidebarVisible();
            SavedAccounts.CollectionChanged += (sender, args) =>
            {
                Console.WriteLine("collectionChanged");
                SaveAccounts();
                ToggleAccountsSidebarVisible();
            };

            AuthCommand = ReactiveCommand.Create(()=>
            {
                       
                        InfoText="Открытие авторизации";
                        Task.Run(() =>
                        {
                            
                            _webElementServer = new WebElement.WebElementServer(2654);
                             _webElementServer.ErrorEvent += WebServer_ErrorEvent;
                            _webElementServer.MessageRecived += WebServer_MessageEvent;
                            _webElementServer.StartServerOnThread();

                           

                            Thread.Sleep(1000);

                            if(_webElementServer.ServerStarted)
                            {
                                string fileExecute = string.Empty;
                                string args= $"{AuthUrl} 2654";
                                

                                if(ETC.GlobalVars.CurrentPlatform == OSPlatform.Linux)
                                    fileExecute =  Path.Combine("WebElement","Linux");

                                else if(ETC.GlobalVars.CurrentPlatform == OSPlatform.Windows)
                                    fileExecute = Path.Combine("WebElement", "WindowsWebBrowser.exe");
                                
                                var start = new ProcessStartInfo
                                {
                                        UseShellExecute = false,
                                        CreateNoWindow = true,
                                        FileName = fileExecute,
                                        Arguments = args,
                                    
                                };

                                _BrowserProcess = new Process { StartInfo = start };
                                
                                   
                                InfoText="Ожидание конца авторизации";
                                   
                                _BrowserProcess.Start();
                                _BrowserProcess.WaitForExit();
                                OffServerAndUnsubscribe();
                                    
                                
                                

                               
                                
                        }
                    });
            });

                    
            
            RemoveAccountCommand = ReactiveCommand.Create((RoutedEventArgs e) =>
            {
                var accountModel = (e.Source as Button)?.DataContext as SavedAccountModel;
                SavedAccounts.Remove(accountModel);
            });
           
          
        }

        public ObservableCollection<SavedAccountModel>? SavedAccounts { get; set; } = new();

        private bool _AuthButtonIsActive;
        public bool AuthButtonIsActive{get=>_AuthButtonIsActive; set=>this.RaiseAndSetIfChanged(ref _AuthButtonIsActive, value);}
        

     
        public string InfoText
        {
            get => _InfoText;
            set => this.RaiseAndSetIfChanged(ref _InfoText, value);
        }

     
        public bool SavedAccountsIsVisible
        {
            get => _SavedAccountsIsVisible;
            set => this.RaiseAndSetIfChanged(ref _SavedAccountsIsVisible, value);
        }

        public int ActiveAccountSelectIndex
        {
            get => _ActiveAccountSelectIndex;
            set => this.RaiseAndSetIfChanged(ref _ActiveAccountSelectIndex, value);
        }

        public IReactiveCommand? AuthCommand { get; set; }
        public IReactiveCommand? RemoveAccountCommand { get; set; }


        private void ToggleAccountsSidebarVisible()
        {
            SavedAccountsIsVisible = SavedAccounts.Count > 0;
        }
        private void OffServerAndUnsubscribe()
        {
            if(_webElementServer!=null)
            {
                _webElementServer.Stop();     
                _webElementServer.ErrorEvent -= WebServer_ErrorEvent;
                _webElementServer.MessageRecived -= WebServer_MessageEvent;
            }
        }
        private void WebServer_ErrorEvent(Exception ex)
        {
                    InfoText = "$Произошла ошибка {ex.Message}";
                    OffServerAndUnsubscribe();
        }
        private void WebServer_MessageEvent(String message)
        {      
                if(message.Contains("#access_token"))
                {
                    string token = message.Split("=")[1].Split("&")[0];
                    string id = message.Split("=")[3].Split("&")[0];
                    
                     _BrowserProcess.Kill();
                    
                    OffServerAndUnsubscribe();
                    InfoText = "Авторизация успешна";
                    var api = Auth(token,long.Parse(id));
                    SaveAccount(api);
                    GlobalVars.VkApi = api;
                    

                }
        }
        private void LoadSavedAccounts()
        {
            try
            {
                if (GlobalVars.CurrentPlatform == OSPlatform.Windows)
                    LoadSavedAccountsOnWindows();

                else if (GlobalVars.CurrentPlatform == OSPlatform.Linux)
                    LoadSavedAccountsOnLinuxOrMac();

                else if (GlobalVars.CurrentPlatform == OSPlatform.OSX) 
                    LoadSavedAccountsOnLinuxOrMac();

                SavedAccounts?.ToList().AsParallel().ForAll(x => x.LoadBitmapAsync());
            }
            catch (Exception)
            {
            }
        }

        private void LoadSavedAccountsOnWindows()
        {
            RegistryKey? key = null;
            try
            {
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true)?.CreateSubKey(GlobalVars.AppName);
                if (key != null)
                {
                    var data = (string?) key.GetValue(GlobalVars.SavedAccountsFileName);
                    if (data is not null)
                        SavedAccounts = JsonConvert.DeserializeObject<ObservableCollection<SavedAccountModel>>(data);
                }
            }
            finally
            {
                key?.Close();
            }
           
           
        }

        private void LoadSavedAccountsOnLinuxOrMac()
        {
            var home = GlobalVars.HomeDirectory;
            if (string.IsNullOrEmpty(home)) return;
            string path = Path.Combine(home, ".config", GlobalVars.AppName, GlobalVars.SavedAccountsFileName);
            if (File.Exists(path))
                SavedAccounts =
                    JsonConvert.DeserializeObject<ObservableCollection<SavedAccountModel>>(File.ReadAllText(path));
        }

        private void SaveAccount(VkApi? vkApi)
        {
           
            var accountEnumerable = SavedAccounts?.ToList().Where(x => x.UserID == vkApi?.UserId);

            foreach (var savedAccountModel in accountEnumerable) SavedAccounts?.Remove(savedAccountModel);
           
            VkNet.Model.RequestParams.AccountSaveProfileInfoParams accountData=null;
            try{
             accountData = vkApi?.Account.GetProfileInfo();
          
            }
            catch(Exception ex){
                InfoText = "Ошибка:"+ex.Message;
            }
            SavedAccounts?.Insert(0,
                new SavedAccountModel
                {
                    Token = vkApi.Token,
                    UserID = vkApi.UserId,
                    Name = $"{accountData.FirstName} {accountData.LastName}"
                });
           
            GlobalVars.CurrentAccount = SavedAccounts.First();
            
        }

        private void SaveAccounts()
        {
            string saveText = JsonConvert.SerializeObject(SavedAccounts);
            if (GlobalVars.CurrentPlatform == OSPlatform.Windows)
                SaveAccountsOnWindows(saveText);
            else if (GlobalVars.CurrentPlatform == OSPlatform.Linux)
                SaveAccountsOnLinuxOrMac(saveText);
            else if (GlobalVars.CurrentPlatform == OSPlatform.OSX) SaveAccountsOnLinuxOrMac(saveText);
        }

        private void SaveAccountsOnWindows(string? data)
        {
            RegistryKey? key = null;
            try
            {
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true)?.CreateSubKey(GlobalVars.AppName);
                key?.SetValue(GlobalVars.SavedAccountsFileName, data ?? string.Empty);
            }
            finally
            {
                key?.Close();
            }
        }

        private void SaveAccountsOnLinuxOrMac(string? data)
        {
            var home = GlobalVars.HomeDirectory;
            if (string.IsNullOrEmpty(home)) return;
            string path = Path.Combine(home, ".config", GlobalVars.AppName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = Path.Combine(path, GlobalVars.SavedAccountsFileName);
            File.WriteAllText(path, data);
        }
     

        private VkApi Auth(string token, long id)
        {
            var api = new VkApi();

            api.Authorize(new ApiAuthParams {AccessToken = token, UserId = id});
            return api;
           

        }
        private void AuthFromActiveAccount(SavedAccountModel account)
        {
            try
            {
               
                GlobalVars.CurrentAccount = account;
                GlobalVars.VkApi =  Auth(account?.Token,  (long) account?.UserID);
                ActiveAccountSelectIndex = -1;
                _ActiveAccountSelectIndex = -1;
            }
            catch (Exception)
            {
                ActiveAccountSelectIndex = -1;
            }
        }


        public virtual  void SelectedItem(object sender,PointerPressedEventArgs args)
        {
               var selectedAccount = (args?.Source as ContentPresenter).Content as SavedAccountModel;
                if (selectedAccount != null) AuthFromActiveAccount(selectedAccount);
        }
        public virtual void Scrolled(object sender, ScrollChangedEventArgs args){

        }
            
    }
    
}



            