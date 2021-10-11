using Microsoft.Win32;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using VKAvaloniaPlayer.ETC;
using VkNet;
using VkNet.Model;
using System.Linq;
using System.Linq.Expressions;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using VKAvaloniaPlayer.Models;
using Button = Avalonia.Controls.Button;

namespace VKAvaloniaPlayer.ViewModels
{
	public class VkLoginControlViewModel : ViewModelBase
	{
		private bool _CodeSendPanelIsVisible;
		private bool _LoginPanelIsVisible = true;
		private bool _SavedAccountsIsVisible;
		private bool _CodeIsSend;
		private string _Login = string.Empty;
		private string _Passwod = string.Empty;
		private string _Code = string.Empty;
		private string _InfoText = string.Empty;
		private int _ActiveAccountSelectIndex = -1;
		public ObservableCollection<Models.SavedAccountModel>? SavedAccounts { get; set; } = new();

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

		private void ToggleAccountsSidebarVisible() =>
			SavedAccountsIsVisible = SavedAccounts.Count > 0;

		public IReactiveCommand SelectedItemCommand { get; set; }

		public VkLoginControlViewModel()
		{
			SelectedItemCommand = ReactiveCommand.Create((PointerPressedEventArgs e) =>
			{
				var selectedAccount = (e?.Source as ContentPresenter).Content as SavedAccountModel;
				if (selectedAccount != null)
					AuthFromActiveAccount(selectedAccount);
			});

			LoadSavedAccounts();
			ToggleAccountsSidebarVisible();
			SavedAccounts.CollectionChanged += (sender, args) =>
			{
				SaveAccounts();
				ToggleAccountsSidebarVisible();
			};

			SendCodeCommand = ReactiveCommand.Create(() => _CodeIsSend = true,
				this.WhenAnyValue(x => x.Code, (code) => !string.IsNullOrEmpty(code)));
			AuthCommand = ReactiveCommand.Create(() => VkAuth(),
				this.WhenAnyValue(x => x.Login, x => x.Password,
					(login, password) => !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password)));

			RemoveAccountCommand = ReactiveCommand.Create((RoutedEventArgs e) =>
			{
				var accountModel = (e.Source as Button)?.DataContext as SavedAccountModel;
				SavedAccounts.Remove(accountModel);
			});
		}

		private void LoadSavedAccounts()
		{
			try
			{
				if (GlobalVars.CurrentPlatform == OSPlatform.Windows) LoadSavedAccountsOnWindows();
				else if (GlobalVars.CurrentPlatform == OSPlatform.Linux) LoadSavedAccountsOnLinuxOrMac();
				else if (GlobalVars.CurrentPlatform == OSPlatform.OSX) LoadSavedAccountsOnLinuxOrMac();

				SavedAccounts?.ToList().AsParallel().ForAll(x => x.LoadBitmapAsync());
			}
			catch (Exception)
			{
				return;
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
					string? data = (string?)key.GetValue(GlobalVars.SavedAccountsFileName);
					if (data is not null)
						SavedAccounts =
							JsonConvert.DeserializeObject<ObservableCollection<Models.SavedAccountModel>>(data);
				}
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				key?.Close();
			}
		}

		private void LoadSavedAccountsOnLinuxOrMac()
		{
			try
			{
				string? home = GlobalVars.HomeDirectory;
				if (string.IsNullOrEmpty(home)) return;
				string path = Path.Combine(home, ".config", GlobalVars.AppName, GlobalVars.SavedAccountsFileName);
				if (File.Exists(path))
					SavedAccounts =
						JsonConvert.DeserializeObject<ObservableCollection<Models.SavedAccountModel>>(
							File.ReadAllText(path));
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void SaveAccount(VkApi? vkApi)
		{
			var accountEnumerable = SavedAccounts.ToList().Where(x => x.UserID == vkApi.UserId);

			foreach (var savedAccountModel in accountEnumerable)
				SavedAccounts.Remove(savedAccountModel);

			var accountData = vkApi.Account.GetProfileInfo();
			SavedAccounts?.Insert(0, new Models.SavedAccountModel()
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
			if (GlobalVars.CurrentPlatform == OSPlatform.Windows) SaveAccountsOnWindows(saveText);
			else if (GlobalVars.CurrentPlatform == OSPlatform.Linux) SaveAccountsOnLinuxOrMac(saveText);
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
			string? home = GlobalVars.HomeDirectory;
			if (string.IsNullOrEmpty(home)) return;
			string path = Path.Combine(home, ".config", GlobalVars.AppName);
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
			path = Path.Combine(path, GlobalVars.SavedAccountsFileName);
			File.WriteAllText(path, data);
		}

		private void VkBrowserAuth()
		{
			string url = @"https://oauth.vk.com/authorize?client_id=6121396&scope=1073737727&redirect_uri=https://oauth.vk.com/blank.html&display=page&response_type=token&revoke=1";
			try
			{
				var p = Process.Start(url);
				p.OutputDataReceived += P_OutputDataReceived;
			}
			catch
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					url = url.Replace("&", "^&");
					Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					Process.Start("xdg-open", url);
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				{
					Process.Start("open", url);
				}
				else
				{
					throw;
				}
			}
		}

		private void VkAuth()
		{
			VkApi? vkApi = new();
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
				finally
				{
					Login = string.Empty;
					Password = string.Empty;
					Code = string.Empty;
					CodeSendPanelIsVisible = false;
					LoginPanelIsVisible = true;
				}
			});
		}

		private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			Debug.WriteLine(e.Data);
		}

		private void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			Debug.WriteLine(e.Data);
		}

		private void AuthFromActiveAccount(Models.SavedAccountModel account)
		{
			try
			{
				VkApi? api = new VkApi();

				api.Authorize(new ApiAuthParams()
				{
					AccessToken = account?.Token,
					UserId = (long)account?.UserID,
				});

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