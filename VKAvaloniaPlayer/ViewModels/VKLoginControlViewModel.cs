using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using VkNet;
using VkNet.Model;

namespace VKAvaloniaPlayer.ViewModels
{
	public class VKLoginControlViewModel : ViewModelBase
	{
		private string _Login = string.Empty;
		private string _Passwod = string.Empty;
		private string _Code = string.Empty;
		private bool _CodeSendPanelIsVisible = false;
		private bool _LoginPanelIsVisible = true;
		private bool _CodeIsSend = false;
		private string _InfoText = string.Empty;
		private int _ActiveAccountSelectIndex;

		public ObservableCollection<Models.SaveAccountModel> SaveAccounts { get; set; } = new ObservableCollection<Models.SaveAccountModel>();

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
				if (_ActiveAccountSelectIndex > -1)
					AuthFromActiveAccount(_ActiveAccountSelectIndex);
			}
		}

		public IReactiveCommand SendCodeCommand { get; set; }
		public IReactiveCommand AuthCommand { get; set; }

		public VKLoginControlViewModel()
		{
			SendCodeCommand = ReactiveCommand.Create(() => _CodeIsSend = true,
				this.WhenAnyValue(x => x.Code, (code) => !string.IsNullOrEmpty(code)));

			AuthCommand = ReactiveCommand.Create(() => VKAuth(),
				this.WhenAnyValue(x => x.Login, x => x.Password, (login, password) => !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password)));
		}

		private void VKAuth()
		{
			VkNet.VkApi vkApi = new();
			InfoText = "";
			System.Text.EncodingProvider provider = System.Text.CodePagesEncodingProvider.Instance;
			System.Text.Encoding.RegisterProvider(provider);

			var AuthAwauter = vkApi.AuthorizeAsync(new ApiAuthParams()
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
			AuthAwauter.OnCompleted(() =>
			{
				try
				{
					AuthAwauter.GetResult();

					StaticObjects.VKApi = vkApi;
				}
				catch (Exception ex)
				{
					InfoText = ex.Message;
				}
			});
		}

		private OSPlatform CheckPlatForm()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return OSPlatform.Windows;
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				return OSPlatform.Linux;
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
				return OSPlatform.FreeBSD;
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return OSPlatform.OSX;
			else throw new InvalidOperationException();
		}

		private void SaveAccount(VkApi vkApi)
		{
			try
			{
				var accountData = vkApi.Account.GetProfileInfo();
				SaveAccounts.Add(new Models.SaveAccountModel() { Token = vkApi.Token, UserID = (long)vkApi.UserId, Name = $"{accountData.FirstName} {accountData.LastName}" });
				File.WriteAllText("Users", JsonConvert.SerializeObject(SaveAccounts));
			}
			catch (Exception) { throw; }
		}

		private void AuthFromActiveAccount(int index)
		{
			try
			{
				var Account = SaveAccounts[index];
				VkApi API = new VkApi();
				API.Authorize(new ApiAuthParams()
				{
					AccessToken = Account.Token,
					UserId = Account.UserID,
				});
				StaticObjects.VKApi = API;
			}
			catch (Exception ex)
			{
				ActiveAccountSelectIndex = -1;
				return;
			}
		}
	}
}