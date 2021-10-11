using Avalonia.Media.Imaging;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using VKAvaloniaPlayer.Models.Interfaces;

namespace VKAvaloniaPlayer.Models.Base
{
	public enum ModelTypes
	{
		Audio,
		Album
	}

	public class VkModelBase : ReactiveUI.ReactiveObject, Interfaces.IVkModelBase

	{
		public ModelTypes ModelType { get; set; }
		public long ID { get; set; }
		public long OwnerID { get; set; }
		public string Artist { get; set; }
		public string Title { get; set; }
		public IImageBase Cover { get; set; }

		public virtual string GetThumbUrl(VkNet.Model.AudioCover audioCover) => string.Empty;
	}
}