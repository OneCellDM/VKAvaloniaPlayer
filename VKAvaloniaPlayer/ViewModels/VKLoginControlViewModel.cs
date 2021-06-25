using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using VkNet.Model;

namespace VKAvaloniaPlayer.ViewModels
{
    public  class VKLoginControlViewModel:ViewModelBase
    {
        private string _Login = string.Empty;
        private string _Passwod = string.Empty;
        private string _Code = string.Empty;
        private bool _CodeSendPanelIsVisible = false;
        private bool _LoginPanelIsVisible = true;
        private bool _CodeIsSend = false;
        private bool _LoginInfoPanelIsVisible = false;
        private string _LoginInfo = string.Empty;
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
        public string LoginInfo { 
            get => _LoginInfo; 
            set => this.RaiseAndSetIfChanged(ref _LoginInfo, value); 
        }
        public bool CodeSendPanelIsVisible 
        {
            get => _CodeSendPanelIsVisible; 
            set => this.RaiseAndSetIfChanged(ref _CodeSendPanelIsVisible,value); 
        }
        public bool LoginPanelIsVisible 
        {
            get => _LoginPanelIsVisible;
            set => this.RaiseAndSetIfChanged(ref _LoginPanelIsVisible, value); 
        }

        public bool LoginInfoPanelIsVisible { 
            get => _LoginInfoPanelIsVisible;
            set => this.RaiseAndSetIfChanged(ref _LoginInfoPanelIsVisible, value);
        }
        public IReactiveCommand SendCodeCommand { get; set; }
        public IReactiveCommand AuthCommand { get; set; }
        public VKLoginControlViewModel()
        {
            SendCodeCommand = ReactiveCommand.Create(() =>_CodeIsSend = true, 
                this.WhenAnyValue(x=>x.Code, (code)=> !string.IsNullOrEmpty(code)));
            
            AuthCommand = ReactiveCommand.Create(() =>
            {
                System.Text.EncodingProvider provider = System.Text.CodePagesEncodingProvider.Instance;
                System.Text.Encoding.RegisterProvider(provider);
                VkNet.VkApi vkApi = new();

                var AuthAwauter = vkApi.AuthorizeAsync(new ApiAuthParams()
                {   
                    Login = this.Login,
                    Password=this.Password,
                   
                    TwoFactorSupported = true,
                    TwoFactorAuthorization = () =>
                    {
                        if ( LoginPanelIsVisible )
                        {
                            CodeSendPanelIsVisible = true;
                            LoginPanelIsVisible = false;
                        }
                        string tmpcode = "";

                        if ( _CodeIsSend )
                        {
                            tmpcode = Code;
                            _CodeIsSend = false;
                            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => Code = string.Empty);
                        }
                        return tmpcode;
                    }

                }).GetAwaiter();
                AuthAwauter.OnCompleted(() =>
                {
                   
                        AuthAwauter.GetResult();
                        StaticObjects.VKApi = vkApi;

                   

                });

                
            },  this.WhenAnyValue(x => x.Login,x => x.Password,  (login, password) => 
                !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password) 
                ));           
        }

    }
}
