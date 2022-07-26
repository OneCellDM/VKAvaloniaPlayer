namespace VKAvaloniaPlayer.Models.Interfaces
{
    public interface IVkModelBase
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public IImageBase Image { get; set; }
    }
}
