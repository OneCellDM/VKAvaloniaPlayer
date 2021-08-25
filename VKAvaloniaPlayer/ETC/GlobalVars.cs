using System.Runtime.InteropServices;
using Avalonia.Media.Imaging;
using VKAvaloniaPlayer.Models;
using VkNet;

namespace VKAvaloniaPlayer.ETC
{
    public class GlobalVars
    {
        private static string? _homedirectory;
        private static OSPlatform? _currentPlatform = null;
        private static VkApi? _vkApi;


        public delegate void Api();

        public static event Api? VkApiChanged;

        public static SavedAccountModel? CurrentAccount { get; set; }

        public static string AppName
        {
            get => "VkAvaloniaPlayer";
        }

        public static string SavedAccountsFileName
        {
            get => "Accounts";
        }

        public static string? HomeDirectory
        {
            get
            {
                if (_homedirectory == null)
                    _homedirectory = Utils.GetHomeDirectory();

                return _homedirectory;
            }
        }

        public static Bitmap? DefaultMusicImage { get; set; }
        public static Bitmap? DefaultAlbumImage { get; set; }

        public static VkApi VkApi
        {
            get
            {
                if (_vkApi is null)
                    _vkApi = new();

                return _vkApi;
            }
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