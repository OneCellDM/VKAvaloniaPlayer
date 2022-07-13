using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.ViewModels.Base;

namespace VKAvaloniaPlayer.ViewModels.Audios
{
    public class AlbumsViewModel : DataViewModelBase <AudioAlbumModel>
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