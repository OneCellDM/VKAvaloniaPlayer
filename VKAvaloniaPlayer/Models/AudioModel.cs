using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VKAvaloniaPlayer.Models
{
  public  class AudioModel:BaseModel
  {
        public int Duration { get; set; }
        public static Bitmap DefaultImage { get; set; }
        public AudioModel ()
        {
            if ( DefaultImage == null )
                DefaultImage = new Bitmap(
                    AvaloniaLocator.Current.GetService<IAssetLoader>()
                        .Open(new Uri(@"Avares://VKAvaloniaPlayer/Assets/MusicIcon.jpg")
                        )
                    );
            Cover = DefaultImage;
            Title = "Название";
            Artist = "Исполнитель";

        }
        
        public  AudioModel(VkNet.Model.Attachments.Audio VkModel)
        {
            ModelType = ModelTypes.Audio;
            Duration = VkModel.Duration;
            ID =(long)VkModel.Id;
            OwnerID = (long)VkModel.OwnerId;
            Artist = VkModel.Artist;
            Title = VkModel.Title;

            if ( VkModel.Album != null && VkModel.Album.Thumb != null )
                    CoverUrl = GetThumbUrl(VkModel.Album.Thumb);
            Cover = DefaultImage;

        }
        
        private string GetThumbUrl(VkNet.Model.AudioCover audioCover)
        {
            if (audioCover.Photo68 != null)
                return audioCover.Photo68;
            if ( audioCover.Photo135 != null )
                return audioCover.Photo135;
            else return string.Empty;
        }
  }
}
