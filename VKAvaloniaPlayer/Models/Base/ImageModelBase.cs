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
using VKAvaloniaPlayer.Models.Interfaces;

namespace VKAvaloniaPlayer.Models.Base
{
	public class ImageModelBase : INotifyPropertyChanged, IImageBase
	{
		public static Semaphore Semaphore = new Semaphore(5, 5);

		public event PropertyChangedEventHandler? PropertyChanged;

		private Bitmap? _Image = null;

		public string ImageUrl { get; set; }

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

		public virtual async void LoadBitmap()
		{
		}

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}
}