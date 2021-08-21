using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VKAvaloniaPlayer.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private const double _menuColumnWidth = 60;
		private bool _MenuTextIsVisible = false;
		private bool _AlbumsIsVisible = false;
		private bool _CurrentDataViewIsVisible = true;
		private bool _VkLoginIsVisible = true;

		private AlbumsViewModel _AlbumsViewModel;

		private DataViewModelBase _CurrentDataViewModel;

		private AllMusicViewModel _AllMusicViewModel;
		private AudioSearchViewModel _AudioSearchViewModel;
		private RecomendationsViewModel _RecomendationsViewModel;

		private Bitmap _Avatar;

		private int _MenuSelectionIndex = -1;

		private GridLength _MenuColumnWidth = new GridLength(_menuColumnWidth);

		private string _UserName = "Загрузка...";

		private bool AnimPlayed = false;

		public bool MenuTextIsVisible
		{
			get => _MenuTextIsVisible;
			set => this.RaiseAndSetIfChanged(ref _MenuTextIsVisible, value);
		}

		public bool AlbumsIsVisible
		{
			get => _AlbumsIsVisible;
			set => this.RaiseAndSetIfChanged(ref _AlbumsIsVisible, value);
		}

		public bool CurrentDataViewIsVisible
		{
			get => _CurrentDataViewIsVisible;
			set => this.RaiseAndSetIfChanged(ref _CurrentDataViewIsVisible, value);
		}

		public bool VkLoginIsVisible
		{
			get => _VkLoginIsVisible;
			set => this.RaiseAndSetIfChanged(ref _VkLoginIsVisible, value);
		}

		public AlbumsViewModel AlbumsViewModel
		{
			get => _AlbumsViewModel;
			set => this.RaiseAndSetIfChanged(ref _AlbumsViewModel, value);
		}

		public DataViewModelBase CurrentDataViewModel
		{
			get => _CurrentDataViewModel;
			set => this.RaiseAndSetIfChanged(ref _CurrentDataViewModel, value);
		}

		public Bitmap Avatar
		{
			get => _Avatar;
			set => this.RaiseAndSetIfChanged(ref _Avatar, value);
		}

		public int MenuSelectionIndex
		{
			get => _MenuSelectionIndex;
			set
			{
				this.RaiseAndSetIfChanged(ref _MenuSelectionIndex, value);
				OpenView(value);
			}
		}

		public GridLength MenuColumnWidth
		{
			get => _MenuColumnWidth;
			set => this.RaiseAndSetIfChanged(ref _MenuColumnWidth, value);
		}

		public string UserName
		{
			get => _UserName;
			set => this.RaiseAndSetIfChanged(ref _UserName, value);
		}

		public IReactiveCommand MenuPointEnterCommand { get; set; }
		public IReactiveCommand MenuPointLeaveCommand { get; set; }

		public void OpenView(int MenuIndex)
		{
			Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
			{
				CurrentDataViewIsVisible = true;
				CurrentDataViewModel = null;
				AlbumsIsVisible = false;

				switch (MenuIndex)
				{
					case 0:
						{
							if (_AllMusicViewModel is null)
							{
								_AllMusicViewModel = new AllMusicViewModel();
								_AllMusicViewModel.LoadData();
							}

							CurrentDataViewModel = _AllMusicViewModel;
							break;
						}

					case 1:
						{
							if (_AlbumsViewModel is null)
							{
								AlbumsViewModel = new AlbumsViewModel();
								AlbumsViewModel.LoadData();
							}

							CurrentDataViewIsVisible = false;
							AlbumsIsVisible = true;
							break;
						}

					case 2:
						{
							if (_AudioSearchViewModel is null)
								_AudioSearchViewModel = new AudioSearchViewModel();

							CurrentDataViewModel = _AudioSearchViewModel;
							break;
						}

					case 3:
						{
							if (_RecomendationsViewModel is null)
							{
								_RecomendationsViewModel = new RecomendationsViewModel();
								_RecomendationsViewModel.LoadData();
							}

							CurrentDataViewModel = _RecomendationsViewModel;
							break;
						}
				}
			});
		}

		private void StaticObjects_VkApiChanged()
		{
			var profileInfoAwaiter = StaticObjects.VKApi.Users.GetAsync(new[] { (long)StaticObjects.VKApi.UserId }, VkNet.Enums.Filters.ProfileFields.Photo50).GetAwaiter();
			profileInfoAwaiter.OnCompleted(() =>
			{
				UserName = profileInfoAwaiter.GetResult()[0].FirstName;
				HttpClient httpClient = new HttpClient();
				Task.Run(async () =>
				{
					Avatar = new Bitmap(
						new MemoryStream(
							await httpClient.GetByteArrayAsync(profileInfoAwaiter.GetResult()[0].Photo50.AbsoluteUri)
						   )
						);
				});
			});

			Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
			{
				VkLoginIsVisible = false;

				MenuSelectionIndex = 0;
			});

			StaticObjects.VkApiChanged -= StaticObjects_VkApiChanged;
		}

		public MainWindowViewModel()
		{
			StaticObjects.VkApiChanged += StaticObjects_VkApiChanged;
			MenuPointEnterCommand = ReactiveCommand.Create((object obj) =>
			{
				MenuColumnWidth = GridLength.Auto;
				MenuTextIsVisible = true;
			});
			MenuPointLeaveCommand = ReactiveCommand.Create((object obj) =>
			{
				MenuTextIsVisible = false;
				MenuColumnWidth = new GridLength(_menuColumnWidth);
			});
		}
	}
}