using VKAvaloniaPlayer.Models.Base;

namespace VKAvaloniaPlayer.Models.Interfaces
{
    public interface IVkModelBase
    {
        public ModelTypes ModelType { get; set; }
        public long ID { get; set; }
        public long OwnerID { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public IImageBase Cover { get; set; }
        public string Subtitle { get; set; }
    }
}