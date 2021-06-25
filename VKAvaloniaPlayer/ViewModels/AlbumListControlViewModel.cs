using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKAvaloniaPlayer.ViewModels
{
    public  class AlbumListControlViewModel:ViewModelBase
    {
        private bool Isloading;
        public ObservableCollection<Models.AudioAlbumModel> AudioAlbums { get; set; }
        public AlbumListControlViewModel ()
        {
            AudioAlbums = new ObservableCollection<Models.AudioAlbumModel>();
        }
        public void Load()
        {
            if ( Isloading == false )
            Task.Run(() => {

                Isloading = true;
                AudioAlbums.LoadFromVkCollection(StaticObjects.VKApi.Audio.GetPlaylists(StaticObjects.VKApi.UserId.Value,200,0) );
                Isloading = false;
            });
        }
    }
}
