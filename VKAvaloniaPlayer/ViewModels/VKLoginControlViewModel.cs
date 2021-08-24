using Microsoft.Win32;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using VKAvaloniaPlayer.ETC;
using VkNet;
using VkNet.Model;

namespace VKAvaloniaPlayer.ViewModels
{
    public class VkLoginControlViewModel : ViewModelBase
    {
        private string _Login = string.Empty;
        private string _Passwod = string.Empty;
        private string _Code = string.Empty;
        private bool _CodeSendPanelIsVisible;
        private bool _LoginPanelIsVisible = true;
        private bool _CodeIsSend;
        private string _InfoText = string.Empty;
        private int _ActiveAccountSelectIndex;
        public ObservableCollection<Models.SaveAccountModel>? SavedAccounts { get; set; } = new();

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

        public int ActiveAccountSelectIndex
        {
            get => _ActiveAccountSelectIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _ActiveAccountSelectIndex, value);
                if (_ActiveAccountSelectIndex > -1) AuthFromActiveAccount(_ActiveAccountSelectIndex);
            }
        }

        public IReactiveCommand? SendCodeCommand { get; set; }
        public IReactiveCommand? AuthCommand { get; set; }

        public VkLoginControlViewModel()
        {
            LoadSavedAccounts();
            SendCodeCommand = ReactiveCommand.Create(() => _CodeIsSend = true,
                this.WhenAnyValue(x => x.Code, (code) => !string.IsNullOrEmpty(code)));
            AuthCommand = ReactiveCommand.Create(() => VkAuth(),
                this.WhenAnyValue(x => x.Login, x => x.Password,
                    (login, password) => !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password)));
        }

        private void VkAuth()
        {
            VkApi vkApi = new();
            InfoText = "";
            System.Text.EncodingProvider provider = System.Text.CodePagesEncodingProvider.Instance;
            System.Text.Encoding.RegisterProvider(provider);
            var authAwauter = vkApi.AuthorizeAsync(new ApiAuthParams()
            {
                ApplicationId = 6121396,
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
                    InfoText = ex.Message;
                }
            });
        }

        private void LoadSavedAccounts()
        {
            if (GlobalVars.CurrentPlatform == OSPlatform.Windows) LoadSavedAccountsOnWindows();
        }

        private void LoadSavedAccountsOnWindows()
        {
            RegistryKey? key = null;
            try
            {
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true)?.CreateSubKey("VkAvaloniaPlayer");
                if (key is not null)
                {
                    string? data = (string?) key.GetValue("Users");
                    if (data is not null)
                        SavedAccounts =
                            JsonConvert.DeserializeObject<ObservableCollection<Models.SaveAccountModel>>(data);
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                key?.Close();
            }
        }

        private void SaveAccountsOnWindows(string? data)
        {
            RegistryKey? key = null;
            try
            {
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true)?.CreateSubKey("VkAvaloniaPlayer");
                key?.SetValue("Users", data ?? string.Empty);
            }
            finally
            {
                key?.Close();
            }
        }

        private void SaveAccountsOnLinux()
        {
        }

        private string LoadSavedAccountsOnLinux()
        {
            return "str";
        }
       
        private void SaveAccount(VkApi vkApi)
        {
            var accountData = vkApi.Account.GetProfileInfo();
            SavedAccounts?.Add(new Models.SaveAccountModel()
            {
                Token = vkApi.Token, UserID = vkApi.UserId, Name = $"{accountData.FirstName} {accountData.LastName}"
            });
            string saveText = JsonConvert.SerializeObject(SavedAccounts);
            if (GlobalVars.CurrentPlatform == OSPlatform.Windows) SaveAccountsOnWindows(saveText);
            else if (GlobalVars.CurrentPlatform == OSPlatform.Linux) SaveAccountsOnLinux();
        }

        private void AuthFromActiveAccount(int index)
        {
            try
            {
                var account = SavedAccounts?[index];
                VkApi api = new VkApi();
                api.Authorize(new ApiAuthParams() {AccessToken = account?.Token, UserId = (long) account?.UserID,});
                GlobalVars.VkApi = api;
            }
            catch (Exception)
            {
                ActiveAccountSelectIndex = -1;
            }
        }
    }
}