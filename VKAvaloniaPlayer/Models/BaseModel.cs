using Avalonia.Media.Imaging;
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace VKAvaloniaPlayer.Models
{
	public enum ModelTypes
	{
		Audio,
		Album
	}

	public class BaseModel : IBaseModel
	{
		private static Semaphore Semaphore = new Semaphore(20, 20);
		private static HttpClient s_httpClient = new();
		private Bitmap? _cover;

		public event PropertyChangedEventHandler? PropertyChanged;

		public ModelTypes ModelType { get; set; }
		public long ID { get; set; }
		public long OwnerID { get; set; }
		public string Artist { get; set; }
		public string Title { get; set; }
		public string CoverUrl { get; set; }
		public Bitmap? Cover { get => _cover; set { _cover = value; OnPropertyChanged(); } }

		private async Task<Stream>? LoadImageStreamAsync()
		{
			try
			{
				var data = await s_httpClient.GetByteArrayAsync(CoverUrl);
				return new MemoryStream(data);
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async void LoadBitmap()
		{
			if (!string.IsNullOrEmpty(CoverUrl))
			{
				try
				{
					Semaphore.WaitOne();

					await using (var imageStream = await LoadImageStreamAsync())
					{
						if (imageStream is null)
							return;
						Cover = await Task.Run(() => ModelType switch
						{
							ModelTypes.Audio => Bitmap.DecodeToWidth(imageStream, 50, Avalonia.Visuals.Media.Imaging.BitmapInterpolationMode.Default),
							ModelTypes.Album => Bitmap.DecodeToWidth(imageStream, 135, Avalonia.Visuals.Media.Imaging.BitmapInterpolationMode.Default),
							_ => throw new System.NotImplementedException(),
						}); ;
					}
				}
				finally { Semaphore.Release(); }
			}
		}

		public virtual string GetThumbUrl(VkNet.Model.AudioCover audioCover) => string.Empty;

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}
}