using VkNet.Model;

namespace VKAvaloniaPlayer.Models
{
	public class AudioAlbumModel : BaseModel
	{
		public AudioAlbumModel(VkNet.Model.Attachments.AudioPlaylist audioPlaylist)
		{
			Cover = StaticObjects.DefaultAlbumImage;
			ModelType = ModelTypes.Album;

			Title = audioPlaylist.Title;

			ID = (long)audioPlaylist.Id;

			OwnerID = (long)audioPlaylist.OwnerId;

			if (audioPlaylist.Photo != null)
				CoverUrl = GetThumbUrl(audioPlaylist.Photo);
		}

		public override string GetThumbUrl(AudioCover audioCover)
		{
			if (audioCover.Photo135 != null)
				return audioCover.Photo135;

			if (audioCover.Photo270 != null)
				return audioCover.Photo270;
			else return string.Empty;
		}
	}
}