using Avalonia.Input;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.ViewModels.Base;

namespace VKAvaloniaPlayer.ViewModels.Audios
{
    public class AlbumsViewModel : DataViewModelBase<AudioAlbumModel>
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


        public override void SelectedItem(object sender, PointerPressedEventArgs args)
        {
            
            var item = args?.GetContent<AudioAlbumModel>();
            if (item != null)
            {
                MusicFromAlbumViewModel = new MusicFromAlbumViewModel(item);
                MusicFromAlbumViewModel.StartLoad();
                MusicFromAlbumIsVisible = true;
            }

        }


        protected override void LoadData()
        {
            if (GlobalVars.CurrentAccount?.UserID != null)
            {
                var res = GlobalVars.VkApi.Audio.GetPlaylists((long)GlobalVars.CurrentAccount.UserID, 200,
                    (uint)Offset);
                if (res != null)
                {
                    DataCollection.AddRange(res);

                    DataCollection.StartLoadImagesAsync();
                }
            }
        }
    }
}