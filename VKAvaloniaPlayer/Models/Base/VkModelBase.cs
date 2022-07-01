using ReactiveUI;

using VKAvaloniaPlayer.Models.Interfaces;

using VkNet.Model;

namespace VKAvaloniaPlayer.Models.Base
{
    public enum ModelTypes
    {
        Audio,
        Album
    }

    public class VkModelBase : ReactiveObject, IVkModelBase

    {
        public ModelTypes ModelType { get; set; }
        public long ID { get; set; }
        public long OwnerID { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public IImageBase Cover { get; set; }
        public string Subtitle { get; set; }

        public virtual string GetThumbUrl(AudioCover audioCover)
        {
            return string.Empty;
        }
    }
}