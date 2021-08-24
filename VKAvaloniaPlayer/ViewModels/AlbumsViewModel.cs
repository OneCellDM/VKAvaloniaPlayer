using ReactiveUI;
using System.Diagnostics;
using System.Threading.Tasks;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.ViewModels.Base;
using VkNet;

namespace VKAvaloniaPlayer.ViewModels
{
	public class AlbumsViewModel : DataViewModelBase
	{
		private MusicFromAlbumViewModel? _MusicFromAlbumViewModel;

		private bool _MusicFromAlbumIsVisible = false;

		public IReactiveCommand BackToAlbumListCommand { get; set; }

		public MusicFromAlbumViewModel? MusicFromAlbumViewModel
		{
			get => _MusicFromAlbumViewModel;

			set => this.RaiseAndSetIfChanged(ref _MusicFromAlbumViewModel, value);
		}

		public bool MusicFromAlbumIsVisible
		{
			get => _MusicFromAlbumIsVisible;
			set => this.RaiseAndSetIfChanged(ref _MusicFromAlbumIsVisible, value);
		}

		public override void SelectedItem()
		{
			if (SelectedIndex > -1 && DataCollection.Count > 0)
			{
				MusicFromAlbumViewModel = new MusicFromAlbumViewModel((Models.AudioAlbumModel)DataCollection[SelectedIndex]);
				MusicFromAlbumViewModel.LoadData();
				MusicFromAlbumIsVisible = true;
			}
		}

		public override void LoadData()
		{
			Task.Run(() =>
			{
				if (GlobalVars.UserID != null)
				{
					var res = GlobalVars.VkApi?.Audio.GetPlaylists((long) GlobalVars.UserID, 200,
						(uint) Offset);
					if (res != null)
					{
						DataCollection.AddRange(res);

						Task.Run(() => DataCollection.StartLoadImages());
					}
				}

				Loading = false;
			});
		}

		public AlbumsViewModel()
		{
			BackToAlbumListCommand = ReactiveCommand.Create(() =>
			{
				MusicFromAlbumIsVisible = false;
				MusicFromAlbumViewModel = null;
				SelectedIndex = -1;
			});
		}
	}
}