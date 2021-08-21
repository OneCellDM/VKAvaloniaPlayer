using ReactiveUI;
using System.Diagnostics;
using System.Threading.Tasks;
using VKAvaloniaPlayer.ETC;

namespace VKAvaloniaPlayer.ViewModels
{
	public class AlbumsViewModel : DataViewModelBase
	{
		private MusicFromAlbumViewModel _MusicFromAlbumViewModel;

		private bool _MusicFromAlbumIsVisible = false;

		public IReactiveCommand BackToAlbumListCommand { get; set; }

		public MusicFromAlbumViewModel MusicFromAlbumViewModel
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
				var Res = StaticObjects.VKApi.Audio.GetPlaylists((long)StaticObjects.VKApi.UserId, 200, (uint)Offset);

				DataCollection.AddRange(Res);

				Task.Run(() => DataCollection.StartLoadImages());

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