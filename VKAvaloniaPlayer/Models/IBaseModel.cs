using Avalonia.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VKAvaloniaPlayer.Models
{
	public interface IBaseModel : INotifyPropertyChanged
	{
		public ModelTypes ModelType { get; set; }
		public long ID { get; set; }
		public long OwnerID { get; set; }
		public string Artist { get; set; }
		public string Title { get; set; }
		public string? CoverUrl { get; set; }
		public Bitmap? Cover { get; set; }

		public void OnPropertyChanged([CallerMemberName] string prop = "");

		public void LoadBitmap();
	}
}