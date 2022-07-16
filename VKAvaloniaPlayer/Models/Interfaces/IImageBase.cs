using Avalonia.Media.Imaging;

using Newtonsoft.Json;

using System.IO;
using System.Threading.Tasks;

namespace VKAvaloniaPlayer.Models.Interfaces
{
    public interface IImageBase
    {
        public string ImageUrl { get; set; }
        public bool ImageIsloaded { get; set; }

        [JsonIgnore] public Bitmap? Bitmap { get; set; }

        public Task<Stream?>? LoadImageStreamAsync();

        public void LoadBitmapAsync()
        {
        }
    }
}