using Avalonia.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VKAvaloniaPlayer.Models.Interfaces
{
	public interface IVkModelBase

	{
		public Base.ModelTypes ModelType { get; set; }
		public long ID { get; set; }
		public long OwnerID { get; set; }
		public string Artist { get; set; }
		public string Title { get; set; }
		public IImageBase Cover { get; set; }
	}
}