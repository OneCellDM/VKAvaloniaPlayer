using Avalonia.Media.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace VKAvaloniaPlayer.Models.Interfaces
{
	public interface IImageBase
	{
		public string ImageUrl { get; set; }
		public Bitmap? Image { get; set; }

		public Task<Stream?>? LoadImageStreamAsync();

		public void LoadBitmap();
	}
}