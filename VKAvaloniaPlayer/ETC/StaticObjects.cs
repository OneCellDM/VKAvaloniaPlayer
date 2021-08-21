using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using VkNet;

namespace VKAvaloniaPlayer
{
	public static class StaticObjects
	{
		public static Bitmap DefaultMusicImage { get; set; }
		public static Bitmap DefaultAlbumImage { get; set; }
		private static VkApi? _VkApi;

		public delegate void Api();

		public static event Api? VkApiChanged;

		static StaticObjects()
		{
			DefaultMusicImage = LoadImageFromAssets("MusicIcon.jpg");
			DefaultAlbumImage = LoadImageFromAssets("AlbumIcon.png");
		}

		public static VkApi? VKApi
		{
			get => _VkApi;
			set
			{
				_VkApi = value;
				if (VkApiChanged != null)
					VkApiChanged.Invoke();
			}
		}

		public static Bitmap LoadImageFromAssets(string path)
		{
			Uri pathUri = new Uri(@"Avares://VKAvaloniaPlayer/Assets/" + path);
			return new Bitmap(AvaloniaLocator.Current.GetService<IAssetLoader>().Open(pathUri));
		}
	}
}