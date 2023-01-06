using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VKAvaloniaPlayer.Models.Interfaces;

using VkNet.Model;

namespace VKAvaloniaPlayer.Models.Base
{

    public class VkAudioOrAlbumModelBase : ReactiveObject, IVkAudiOrAlbumModelBase

    {
        public long ID { get; set; }
        public long OwnerID { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public IImageBase Image { get; set; }
        public string Subtitle { get; set; }
        public string AccessKey { get; set; }
        
        [Reactive]
        public bool IsNotAvailable { get; set; }

        public virtual string GetThumbUrl(AudioCover audioCover)
        {
            return string.Empty;
        }
    }
}