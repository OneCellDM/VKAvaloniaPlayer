using ReactiveUI;

using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.ViewModels.Base;

namespace VKAvaloniaPlayer.ViewModels
{
    public class AlbumsViewModel : VkDataViewModelBase
    {
        private bool _MusicFromAlbumIsVisible;
        private MusicFromAlbumViewModel? _MusicFromAlbumViewModel;

        public AlbumsViewModel()
        {
            BackToAlbumListCommand = ReactiveCommand.Create(() =>
            {
                MusicFromAlbumIsVisible = false;
                MusicFromAlbumViewModel = null;
                SelectedIndex = -1;
            });
        }

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
                MusicFromAlbumViewModel = new MusicFromAlbumViewModel((AudioAlbumModel)DataCollection[SelectedIndex]);
                MusicFromAlbumViewModel.StartLoad();
                MusicFromAlbumIsVisible = true;
            }
        }

        public override void LoadData()
        {
            if (GlobalVars.CurrentAccount?.UserID != null)
            {
                var res = GlobalVars.VkApi.Audio.GetPlaylists((long)GlobalVars.CurrentAccount.UserID, 200,
                    (uint)Offset);
                if (res != null)
                {
                    DataCollection.AddRange(res);

                    Task.Run(() => { DataCollection.StartLoadImages(); });
                }
            }
        }
    }
}