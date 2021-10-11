using Avalonia.Media.Imaging;
using System.IO;
using System.Threading.Tasks;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models.Base;
using VkNet.Model;
using ReactiveUI;

namespace VKAvaloniaPlayer.Models
{
	public class AudioModel : VkModelBase
	{
		private bool _IsDownload = false;
		private int _DownloadPercent;

		public int DownloadPercent { get => _DownloadPercent; set => this.RaiseAndSetIfChanged(ref _DownloadPercent, value); }

		public bool IsDownload
		{
			get => _IsDownload;
			set
			{
				if (value == false)
					DownloadPercent = 0;

				this.RaiseAndSetIfChanged(ref _IsDownload, value);
			}
		}
		

		public int Duration { get; set; }
		public string AccessKey{ get; set; }

		public AudioModel()
		{
			Title = "Название";
			Artist = "Исполнитель";
			Cover = new ImageModelBase() { Image = GlobalVars.DefaultMusicImage };
		}

		public AudioModel(VkNet.Model.Attachments.Audio VkModel)
		{
			Cover = new ImageModelBase()
			{
				DecodeWidth = 50,
				Image = GlobalVars.DefaultMusicImage,
			};
			AccessKey = VkModel.AccessKey;

			ModelType = ModelTypes.Audio;

			Duration = VkModel.Duration;

			ID = (long)VkModel.Id;

			OwnerID = (long)VkModel.OwnerId;

			Artist = VkModel.Artist;

			Title = VkModel.Title;

			if (VkModel.Album != null && VkModel.Album.Thumb != null)
				Cover.ImageUrl = GetThumbUrl(VkModel.Album.Thumb);
		}

		public override string GetThumbUrl(AudioCover audioCover)
		{
			if (audioCover.Photo68 != null)
				return audioCover.Photo68;

			if (audioCover.Photo135 != null)
				return audioCover.Photo135;
			else return string.Empty;
		}
	}
}