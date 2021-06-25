using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKAvaloniaPlayer.Models
{
    public class AudioAlbumModel:BaseModel
    {
        
        public static Bitmap DefaultImage { get; set; }
        public AudioAlbumModel (VkNet.Model.Attachments.AudioPlaylist audioPlaylist)
        {
            ModelType = ModelTypes.Album;

            if ( DefaultImage == null )
                DefaultImage = new Bitmap(
                    AvaloniaLocator.Current.GetService<IAssetLoader>()
                        .Open(new Uri(@"Avares://VKAvaloniaPlayer/Assets/AlbumIcon.png")
                        )
                    );
            Cover = DefaultImage;
           
        
            Title = audioPlaylist.Title;
            ID = ( long ) audioPlaylist.Id;
            OwnerID = ( long ) audioPlaylist.OwnerId;

            if ( audioPlaylist.Photo != null  )
                CoverUrl = GetThumbUrl(audioPlaylist.Photo);
            Cover = DefaultImage;
        }

        private string GetThumbUrl ( VkNet.Model.AudioCover audioCover )
        {
          
            if ( audioCover.Photo135 != null )
                return audioCover.Photo135;

            if ( audioCover.Photo270 != null )
                return audioCover.Photo270;

            else return string.Empty;
        }
    }
}
