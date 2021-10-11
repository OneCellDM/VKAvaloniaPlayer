using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VKAvaloniaPlayer.Models.Interfaces;
using Avalonia.Visuals.Media.Imaging;

namespace VKAvaloniaPlayer.Models.Base
{
	public class ImageModelBase : INotifyPropertyChanged, IImageBase
	{
		public static Semaphore _Semaphore = new Semaphore(5, 5);

		public event PropertyChangedEventHandler? PropertyChanged;

		[JsonIgnore]
		private Bitmap? _Image = null;

		public int DecodeWidth { get; set; }

		public string ImageUrl { get; set; }
		public bool ImageIsloaded { get; set; }

		[JsonIgnore]
		public Bitmap? Image
		{
			get => _Image; set
			{
				_Image = value;
				OnPropertyChanged();
			}
		}

		public async Task<Stream?>? LoadImageStreamAsync()
		{
			try
			{
				if (string.IsNullOrEmpty(ImageUrl))
					return null;

				byte[]? bytes = null;

				using (HttpClient httpClient = new HttpClient())
					bytes = await httpClient.GetByteArrayAsync(ImageUrl);

				return new MemoryStream(bytes);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public virtual async void LoadBitmapAsync()
		{
			if ((!string.IsNullOrEmpty(ImageUrl)) && !ImageIsloaded)
			{
				try
				{
					_Semaphore.WaitOne();
					using (var imageStream = await LoadImageStreamAsync())
					{
						if (imageStream is null)
							return;

						Image = await Task.Run(() =>
							 DecodeWidth <= 0 ? new Bitmap(imageStream) :
							 Bitmap.DecodeToWidth(imageStream, DecodeWidth)
						);

						ImageIsloaded = true;
					}
				}
				finally
				{
					_Semaphore.Release();
				}
			}
		}

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}
}