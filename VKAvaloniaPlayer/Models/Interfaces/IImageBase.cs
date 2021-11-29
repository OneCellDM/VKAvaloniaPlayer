using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;

namespace VKAvaloniaPlayer.Models.Interfaces
{
    public interface IImageBase
    {
        public string ImageUrl { get; set; }
        public bool ImageIsloaded { get; set; }

        [JsonIgnore] public Bitmap? Image { get; set; }

        public Task<Stream?>? LoadImageStreamAsync();

        public void LoadBitmapAsync()
        {
        }
    }
}