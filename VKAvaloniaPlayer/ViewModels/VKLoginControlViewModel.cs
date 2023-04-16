using Avalonia.Controls;
using Avalonia.Input;

using Microsoft.Win32;

using Newtonsoft.Json;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;

using VkNet;
using VkNet.Model;

namespace VKAvaloniaPlayer.ViewModels
{
    public class VkLoginControlViewModel : ViewModelBase
    {
        const int Port = 2654;
        bool WaitStartServer = false;
        private const string AuthUrl =
           @"https://oauth.vk.com/oauth/authorize?client_id=6463690" +
           "&scope=1073737727" +
           "&redirect_uri=https://oauth.vk.com/blank.html" +
           "&display=mobile" +
           "&response_type=token" +
           "&revoke=1";

        private Process? _BrowserProcess;

        CancellationToken AuthCancelletionSource = new CancellationToken();

        private WebElement.WebElementServer? _webElementServer;

        public VkLoginControlViewModel()
        {
            
            LoadSavedAccounts();
            SkipMenuIfOnlyOneAccount();
            ToggleAccountsSidebarVisible();
            SavedAccounts.CollectionChanged += (sender, args) =>
            {
                SaveAccounts();
                ToggleAccountsSidebarVisible();
            };

            AuthCommand = ReactiveCommand.Create(() =>
            {

                InfoText = "Открытие авторизации";
                AuthCancelletionSource.ThrowIfCancellationRequested();
                
                Task.Run(async () =>
                {
                    try
                    {
                      
                       
                        _webElementServer = new WebElement.WebElementServer(Port);
                        _webElementServer.ErrorEvent += WebServer_ErrorEvent;
                        _webElementServer.MessageRecived += WebServer_MessageEvent;
                        _webElementServer.StartServerOnThread();


                        while (_webElementServer.ServerStarted == false)
                        {
                            await Task.Delay(1000);
                        }
                        if (_webElementServer.ServerStarted)
                        {
                            string winName = "WindowsWebBrowser.exe";
                            string LinuxName = "Linux";
                            string tmpfileEexecute = string.Empty;
                            string tmpfileEexecute2 = string.Empty;
                            string fileExecute = string.Empty;
                            string args = $"{AuthUrl} {Port}";


                            if (ETC.GlobalVars.CurrentPlatform == OSPlatform.Linux)
                            {
                                tmpfileEexecute = Path.Combine("WebElement", LinuxName);
                                tmpfileEexecute2 = LinuxName;
                            }

                            else if (ETC.GlobalVars.CurrentPlatform == OSPlatform.Windows)
                            {
                               tmpfileEexecute = Path.Combine("WebElement", winName);
                               tmpfileEexecute2 = winName;
                            }

                            if (File.Exists(tmpfileEexecute))
                            {
                                fileExecute = tmpfileEexecute;
                            }
                            else if (File.Exists(tmpfileEexecute2))
                            {
                                fileExecute = tmpfileEexecute2;
                            }


                            var start = new ProcessStartInfo
                            {
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                FileName = fileExecute,
                                Arguments = args,

                            };

                            _BrowserProcess = new Process { StartInfo = start };


                            InfoText = "Ожидание конца авторизации";

                            _BrowserProcess.Start();
                            _BrowserProcess.WaitForExit();
                            OffServerAndUnsubscribe();

                        }
                    }
                    catch (Exception ex)
                    {
                        InfoText = "Ошибка:" + ex.Message;
                    }

                },AuthCancelletionSource);
            });



            RemoveAccountCommand = ReactiveCommand.Create<SavedAccountModel>(account =>
            {
                if (account != null)
                    SavedAccounts.Remove(account);
            });


        }

        public ObservableCollection<SavedAccountModel>? SavedAccounts { get; set; } = new();

        [Reactive]
        public bool AuthButtonIsActive { get; set; }


        [Reactive]
        public string? InfoText { get; set; }


        [Reactive]
        public bool SavedAccountsIsVisible { get; set; }
        [Reactive]
        public int ActiveAccountSelectIndex { get; set; }


        public IReactiveCommand? AuthCommand { get; set; }
        public IReactiveCommand? RemoveAccountCommand { get; set; }


        private void ToggleAccountsSidebarVisible()
        {
            SavedAccountsIsVisible = SavedAccounts?.Count > 0;
        }
        private void OffServerAndUnsubscribe()
        {
            if (_webElementServer != null)
            {
                _webElementServer.Stop();
                _webElementServer.ErrorEvent -= WebServer_ErrorEvent;
                _webElementServer.MessageRecived -= WebServer_MessageEvent;
            }
        }
        private void WebServer_ErrorEvent(Exception ex)
        {
           
            InfoText = $"Произошла ошибка {ex.Message}";
            
           
            OffServerAndUnsubscribe();
        }
        private void WebServer_MessageEvent(String message)
        {
            if (message.Contains("#access_token"))
            {
                string token = message.Split("=")[1].Split("&")[0];
                string id = message.Split("=")[3].Split("&")[0];

                _BrowserProcess?.Kill();

                OffServerAndUnsubscribe();
                InfoText = "Авторизация успешна";
                var api = Auth(token, long.Parse(id));
                SaveAccount(api);
                GlobalVars.VkApi = api;


            }
        }

        private void SkipMenuIfOnlyOneAccount()
        {
            if (SavedAccounts?.Count == 1)
            {
                AuthFromActiveAccount(SavedAccounts.First());
            }
        }

        private void LoadSavedAccounts()
        {
            try
            {
                if (GlobalVars.CurrentPlatform == OSPlatform.Windows)
                    LoadSavedAccountsFromRegistry();

                else LoadSavedAccountsFromConfig();

                SavedAccounts?.ToList().AsParallel().ForAll(x => x.LoadAvatar());
            }
            catch (Exception)
            {
            }
        }

        private void LoadSavedAccountsFromRegistry()
        {
            RegistryKey? key = null;
            try
            {
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true)?.CreateSubKey(GlobalVars.AppName);
                if (key != null)
                {
                    var data = (string?)key.GetValue(GlobalVars.SavedAccountsFileName);
                    if (data is not null)
                        SavedAccounts = JsonConvert.DeserializeObject<ObservableCollection<SavedAccountModel>>(data);
                }
            }
            finally
            {
                key?.Close();
            }


        }

        private void LoadSavedAccountsFromConfig()
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
            
            if( accountEnumerable !=null )
                foreach (var savedAccountModel in accountEnumerable) SavedAccounts?.Remove(savedAccountModel);

            VkNet.Model.RequestParams.AccountSaveProfileInfoParams accountData = null;
            try
            {
                accountData = vkApi?.Account?.GetProfileInfo();

            }
            catch (Exception ex)
            {
                InfoText = "Ошибка:" + ex.Message;
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
                SaveAccountsOnRegistry(saveText);
            else SaveAccountsOnConfig(saveText);

        }

        private void SaveAccountsOnRegistry(string? data)
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

        private void SaveAccountsOnConfig(string? data)
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

            api.Authorize(new ApiAuthParams { AccessToken = token, UserId = id });
            return api;
        }

        private void AuthFromActiveAccount(SavedAccountModel account)
        {
            try
            {

                GlobalVars.CurrentAccount = account;
                GlobalVars.VkApi = Auth(account?.Token, (long)account?.UserID);
                ActiveAccountSelectIndex = -1;

            }
            catch (Exception)
            {
                ActiveAccountSelectIndex = -1;
            }
        }


        public virtual void SelectedItem(object sender, PointerPressedEventArgs args)
        {
            var selectedAccount = args.GetContent<SavedAccountModel>();
            if (selectedAccount != null) AuthFromActiveAccount(selectedAccount);
        }
        public virtual void Scrolled(object sender, ScrollChangedEventArgs args)
        {

        }

    }

}



