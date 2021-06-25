using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Utils;

namespace VKAvaloniaPlayer
{
    public static  class StaticObjects
    {
        private static VkApi _VkApi;
        public delegate void Api();
        public static event Api VkApiChanged;
        public static VkApi VKApi {
            get => _VkApi;
            set { 
                _VkApi = value;
                if (VkApiChanged != null)
                    VkApiChanged.Invoke();
            }
        }

        public static void LoadFromVkCollection ( this ObservableCollection<Models.AudioModel> listCollection,  
            VkCollection<VkNet.Model.Attachments.Audio> VkCollection )
        {
            for ( int i = 0; i < VkCollection.Count; i++ )
                listCollection.Add(new Models.AudioModel(VkCollection [i]));

            for ( int i = 0; i < listCollection.Count; i++ )
                listCollection [i].LoadBitmap();

        }
        public static void LoadFromVkCollection ( this ObservableCollection<Models.AudioAlbumModel> listCollection,
        VkCollection<VkNet.Model.Attachments.AudioPlaylist> VkCollection )
        {
            for ( int i = 0; i < VkCollection.Count; i++ )
                listCollection.Add(new Models.AudioAlbumModel(VkCollection[i]));

            for ( int i = 0; i < listCollection.Count; i++ )
                  listCollection [i].LoadBitmap();

        }



    }
}
