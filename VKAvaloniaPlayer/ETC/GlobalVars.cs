using System.Runtime.InteropServices;
using Avalonia.Media.Imaging;
using VkNet;

namespace VKAvaloniaPlayer.ETC
{
    public class GlobalVars
    {
        private static OSPlatform? _currentPlatform = null;
        private static VkApi? _vkApi;
        
        public delegate void Api();
        public static event Api? VkApiChanged;
        
        public  static  long? UserID
        {
            get => _vkApi.UserId;
        }
        public static Bitmap? DefaultMusicImage { get; set; }
        public static Bitmap? DefaultAlbumImage { get; set; }
        
        public static VkApi? VkApi
        {
            get => _vkApi;
            set
            {
                _vkApi = value;
                if (VkApiChanged != null) VkApiChanged.Invoke();
            }
        }

        public static OSPlatform? CurrentPlatform
        {
            get
            {
                if (_currentPlatform is null) _currentPlatform = Utils.CheckPlatForm();
                return _currentPlatform;
            }
        }
        
        public GlobalVars()
        {
            DefaultMusicImage = Utils.LoadImageFromAssets("MusicIcon.jpg");
            DefaultAlbumImage = Utils.LoadImageFromAssets("AlbumIcon.png");
        }
    }
}