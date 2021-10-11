using VKAvaloniaPlayer.ETC;
using VkNet.Model;
using VKAvaloniaPlayer.Models.Base;

namespace VKAvaloniaPlayer.Models
{
	public class AudioAlbumModel : VkModelBase
	{
		public bool IsFollowing { get; set; }

		public AudioAlbumModel(VkNet.Model.Attachments.AudioPlaylist audioPlaylist)
		{
			Cover = new ImageModelBase()
			{
				Image = GlobalVars.DefaultAlbumImage,
				DecodeWidth = 0
			};
			ModelType = ModelTypes.Album;

			Title = audioPlaylist.Title;

			ID = (long)audioPlaylist.Id;

			OwnerID = (long)audioPlaylist.OwnerId;

			IsFollowing = audioPlaylist.IsFollowing;

			if (audioPlaylist.Photo != null)
				Cover.ImageUrl = GetThumbUrl(audioPlaylist.Photo);
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