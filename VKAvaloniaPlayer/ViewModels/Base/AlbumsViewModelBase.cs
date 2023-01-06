using Avalonia.Input;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.ViewModels.Base;

namespace VKAvaloniaPlayer.ViewModels.Audios
{
    public abstract class AlbumsViewModel : DataViewModelBase<AudioAlbumModel>
    {

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

        [Reactive]
        public MusicFromAlbumViewModel? MusicFromAlbumViewModel { get; set; }

        [Reactive]
        public bool MusicFromAlbumIsVisible { get; set; }

    }
}