using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models.Base;
using VkNet.Model;

namespace VKAvaloniaPlayer.Models
{
	public class AudioModel : Base.VkModelBase
	{
		public int Duration { get; set; }

		public AudioModel()
		{
			Title = "Название";
			Artist = "Исполнитель";
			Image = GlobalVars.DefaultMusicImage;
		}

		public AudioModel(VkNet.Model.Attachments.Audio VkModel)
		{
			Image = GlobalVars.DefaultMusicImage;

			ModelType = ModelTypes.Audio;

			Duration = VkModel.Duration;

			ID = (long)VkModel.Id;

			OwnerID = (long)VkModel.OwnerId;

			Artist = VkModel.Artist;

			Title = VkModel.Title;

			if (VkModel.Album != null && VkModel.Album.Thumb != null)
				ImageUrl = GetThumbUrl(VkModel.Album.Thumb);
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