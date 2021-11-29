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
using VkNet.Exception;
using VkNet.Model;
using VkNet.Utils;
using Button = Avalonia.Controls.Button;

namespace VKAvaloniaPlayer.ViewModels
{
    public class VkLoginControlViewModel : ViewModelBase
    {
        private int _ActiveAccountSelectIndex = -1;
        private string _Code = string.Empty;
        private bool _CodeIsSend;
        private bool _CodeSendPanelIsVisible;
        private string _InfoText = string.Empty;
        private string _Login = string.Empty;
        private bool _LoginPanelIsVisible = true;
        private string _Passwod = string.Empty;
        private bool _SavedAccountsIsVisible;

        public VkLoginControlViewModel()
        {
            SelectedItemCommand = ReactiveCommand.Create((PointerPressedEventArgs e) =>
            {
                var selectedAccount = (e?.Source as ContentPresenter).Content as SavedAccountModel;
                if (selectedAccount != null) AuthFromActiveAccount(selectedAccount);
            });

            LoadSavedAccounts();
            ToggleAccountsSidebarVisible();
            SavedAccounts.CollectionChanged += (sender, args) =>
            {
                SaveAccounts();
                ToggleAccountsSidebarVisible();
            };

            SendCodeCommand = ReactiveCommand.Create(() => { 
                _CodeIsSend = true; 
                VkAuth();  
            },this.WhenAnyValue<VkLoginControlViewModel, bool, string>(x => x.Code,
                    code => !string.IsNullOrEmpty(code)));

            AuthCommand = ReactiveCommand.Create(() => VkAuth(),
                this.WhenAnyValue(x => x.Login, x => x.Password,
                    (login, password) => !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password)));

            RemoveAccountCommand = ReactiveCommand.Create((RoutedEventArgs e) =>
            {
                var accountModel = (e.Source as Button)?.DataContext as SavedAccountModel;
                SavedAccounts.Remove(accountModel);
            });
        }

        public ObservableCollection<SavedAccountModel>? SavedAccounts { get; set; } = new();

        public string Login
        {
            get => _Login;
            set => this.RaiseAndSetIfChanged(ref _Login, value);
        }

        public string Password
        {
            get => _Passwod;
            set => this.RaiseAndSetIfChanged(ref _Passwod, value);
        }

        public string Code
        {
            get => _Code;
            set => this.RaiseAndSetIfChanged(ref _Code, value);
        }

        public string InfoText
        {
            get => _InfoText;
            set => this.RaiseAndSetIfChanged(ref _InfoText, value);
        }

        public bool CodeSendPanelIsVisible
        {
            get => _CodeSendPanelIsVisible;
            set => this.RaiseAndSetIfChanged(ref _CodeSendPanelIsVisible, value);
        }

        public bool LoginPanelIsVisible
        {
            get => _LoginPanelIsVisible;
            set => this.RaiseAndSetIfChanged(ref _LoginPanelIsVisible, value);
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

        public IReactiveCommand? SendCodeCommand { get; set; }
        public IReactiveCommand? AuthCommand { get; set; }
        public IReactiveCommand? RemoveAccountCommand { get; set; }

        public IReactiveCommand SelectedItemCommand { get; set; }

        public IReactiveCommand UrlParseCommand { get; set; }
        public IReactiveCommand StartAuthBrowserCommand { get; set; }

        private void ToggleAccountsSidebarVisible()
        {
            SavedAccountsIsVisible = SavedAccounts.Count > 0;
        }

        private void LoadSavedAccounts()
        {
            try
            {
                if (GlobalVars.CurrentPlatform == OSPlatform.Windows)
                    LoadSavedAccountsOnWindows();
                else if (GlobalVars.CurrentPlatform == OSPlatform.Linux)
                    LoadSavedAccountsOnLinuxOrMac();
                else if (GlobalVars.CurrentPlatform == OSPlatform.OSX) LoadSavedAccountsOnLinuxOrMac();

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
            var accountEnumerable = SavedAccounts.ToList().Where(x => x.UserID == vkApi.UserId);

            foreach (var savedAccountModel in accountEnumerable) SavedAccounts.Remove(savedAccountModel);

            var accountData = vkApi.Account.GetProfileInfo();
            SavedAccounts?.Insert(0,
                new SavedAccountModel
                {
                    Token = vkApi.Token,
                    UserID = vkApi.UserId,
                    Name = $"{accountData.FirstName} {accountData.LastName}"
                });

            GlobalVars.CurrentAccount = SavedAccounts.Last();
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
        private void ShowCodePanel(){
            CodeSendPanelIsVisible = true;
            LoginPanelIsVisible = false;
        }
        private void ShowLoginPanel(){
            CodeSendPanelIsVisible = false;
            LoginPanelIsVisible = true;
        }
        private async void VkAuth()
        {
            var AuthUrl = "https://oauth.vk.com/token?" + $"username={Login}" + $"&password={Password}" +
                          "&grant_type=password" + "&2fa_supported=1" + "&client_secret=L3yBidmMBtFRKO9hPCgF" +
                          "&client_id=6121396";

            var SendCodeUrl = $"{AuthUrl}&code={Code}";

            JObject ResObject;
            
            try
            {     
                if (_CodeIsSend)
                {
                   ResObject = await VkRequest(SendCodeUrl);
                   Code = string.Empty;
                }

                else ResObject = await VkRequest(AuthUrl);

                if (ResObject.ContainsKey("access_token"))
                {
                        VkApi vkApi = new VkApi();
                        vkApi.Authorize(new ApiAuthParams()
                        {
							AccessToken = ResObject["access_token"].ToObject<string>(),
                            UserId = ResObject["user_id"].ToObject<long>()
                        });
                        SaveAccount(vkApi);
                        GlobalVars.VkApi = vkApi;
                        Login = string.Empty;
                        Password = string.Empty;
                }
                else
                {
                    InfoText = "Произошла непредвиденная ошибка при авторизации";
                    _CodeIsSend = false;
                }
                   
            }
            catch (NeedValidationException ex)
            {
                ShowCodePanel();
            }
           
            catch (Exception ex)
            {
                InfoText = ex.Message;
                _CodeIsSend = false;
                Code = string.Empty;
                ShowLoginPanel();
            }    
        }

        public async Task<JObject> VkRequest(string url)
        {
            Utils.HttpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            Utils.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var res = await Utils.HttpClient.GetAsync(url);
            Utils.HttpClient.DefaultRequestHeaders.Clear();

            var awaitdata = await res.Content.ReadAsStringAsync();
            var obj = JObject.Parse(awaitdata);

            if (obj.ContainsKey("error") is false)
                return obj;

            switch (obj["error"].ToObject<string>())
            {
                case "need_captcha":
                    throw new CaptchaNeededException(new VkError()
                    {
                        ErrorCode = VkErrorCode.CaptchaNeeded,
                        CaptchaSid = obj["captcha_sid"].ToObject<ulong>(),
                        CaptchaImg = obj["captcha_img"].ToObject<Uri>(),
                    });

                case "need_validation":
                    throw new NeedValidationException(new VkError());

                default:
                    throw new VkAuthorizationException(obj["error_description"].ToObject<string>());
            }
        }

        private void AuthFromActiveAccount(SavedAccountModel account)
        {
            try
            {
                var api = new VkApi();

                api.Authorize(new ApiAuthParams {AccessToken = account?.Token, UserId = (long) account?.UserID});

                GlobalVars.CurrentAccount = account;
                GlobalVars.VkApi = api;
                ActiveAccountSelectIndex = -1;
                _ActiveAccountSelectIndex = -1;
            }
            catch (Exception)
            {
                ActiveAccountSelectIndex = -1;
            }
        }
    }
}

/*	var authAwauter = vkApi.AuthorizeAsync(new ApiAuthParams()
			{
				ApplicationId = 5776857,
				Display = VkNet.Enums.SafetyEnums.Display.Mobile,

				Login = Login,
				Password = Password,
				TwoFactorSupported = true,
				TwoFactorAuthorization = () =>
				{
					if (LoginPanelIsVisible)
					{
						CodeSendPanelIsVisible = true;
						LoginPanelIsVisible = false;
					}

					string tmpcode = "";
					if (_CodeIsSend)
					{
						tmpcode = Code;
						_CodeIsSend = false;
						Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => Code = string.Empty);
					}

					return tmpcode;
				}
			}).GetAwaiter();

			authAwauter.OnCompleted(() =>
			{
				try
				{
					authAwauter.GetResult();
					SaveAccount(vkApi);
					GlobalVars.VkApi = vkApi;
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
					Debug.WriteLine(ex.StackTrace);
					InfoText = ex.Message;
				}
				finally
				{
					Login = string.Empty;
					Password = string.Empty;
					Code = string.Empty;
					CodeSendPanelIsVisible = false;
					LoginPanelIsVisible = true;
				}
			});
			*/