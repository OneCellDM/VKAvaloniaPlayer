using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models.Base;

using VkNet.Model;
using VkNet.Model.Attachments;

namespace VKAvaloniaPlayer.Models
{
    public class AudioAlbumModel : VkAudioOrAlbumModelBase
    {
        public AudioAlbumModel(AudioPlaylist audioPlaylist)
        {
            Image = new ImageModel
            {
                Bitmap = GlobalVars.DefaultAlbumImage,
                DecodeWidth = 0
            };


            Title = audioPlaylist.Title;

            ID = (long)audioPlaylist.Id;

            OwnerID = (long)audioPlaylist.OwnerId;

            IsFollowing = audioPlaylist.IsFollowing;

            if (audioPlaylist.Photo != null)
                Image.ImageUrl = GetThumbUrl(audioPlaylist.Photo);
        }

        public bool IsFollowing { get; set; }

        public override string GetThumbUrl(AudioCover audioCover)
        {
            if (audioCover.Photo135 != null)
                return audioCover.Photo135;

            if (audioCover.Photo270 != null)
                return audioCover.Photo270;
            return string.Empty;
        }
    }
}