using ReactiveUI;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models.Base;
using VkNet.Model;
using VkNet.Model.Attachments;

namespace VKAvaloniaPlayer.Models
{
    public class AudioModel : VkModelBase
    {
        private int _DownloadPercent;
        private bool _IsDownload;

        public AudioModel()
        {
            Title = "Название";
            Artist = "Исполнитель";
            Cover = new ImageModelBase {Image = GlobalVars.DefaultMusicImage};
        }

        public AudioModel(Audio VkModel)
        {
            Cover = new ImageModelBase
            {
                DecodeWidth = 50,
                Image = GlobalVars.DefaultMusicImage
            };
            AccessKey = VkModel.AccessKey;
            ModelType = ModelTypes.Audio;
            Duration = VkModel.Duration;
            ID = (long) VkModel.Id;
            OwnerID = (long) VkModel.OwnerId;
            Artist = VkModel.Artist;
            Title = VkModel.Title;
            Subtitle = VkModel.Subtitle;

            if (VkModel.Album != null && VkModel.Album.Thumb != null)
                Cover.ImageUrl = GetThumbUrl(VkModel.Album.Thumb);
        }

        public int DownloadPercent
        {
            get => _DownloadPercent;
            set => this.RaiseAndSetIfChanged(ref _DownloadPercent, value);
        }

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
        public string AccessKey { get; set; }

        public override string GetThumbUrl(AudioCover audioCover)
        {
            if (audioCover.Photo68 != null)
                return audioCover.Photo68;

            if (audioCover.Photo135 != null)
                return audioCover.Photo135;
            return string.Empty;
        }
    }
}