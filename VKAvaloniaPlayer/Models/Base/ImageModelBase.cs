using Avalonia.Media.Imaging;

using Newtonsoft.Json;

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models.Interfaces;

namespace VKAvaloniaPlayer.Models.Base
{
    public class ImageModelBase : INotifyPropertyChanged, IImageBase
    {
        public static Semaphore _Semaphore = new(5, 5);

        [JsonIgnore] private Bitmap? _Image;

        public int DecodeWidth { get; set; }

        public string ImageUrl { get; set; }
        public bool ImageIsloaded { get; set; }

        [JsonIgnore]
        public Bitmap? Image
        {
            get => _Image;
            set
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


                bytes = await Utils.HttpClient.GetByteArrayAsync(ImageUrl);

                return new MemoryStream(bytes);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public virtual async void LoadBitmapAsync()
        {
            if (!string.IsNullOrEmpty(ImageUrl) && !ImageIsloaded)
                try
                {
                    _Semaphore.WaitOne();
                    using (var imageStream = await LoadImageStreamAsync())
                    {
                        if (imageStream is null)
                            return;

                        Image = await Task.Run(() =>
                            DecodeWidth <= 0 ? new Bitmap(imageStream) : Bitmap.DecodeToWidth(imageStream, DecodeWidth)
                        );

                        ImageIsloaded = true;
                    }
                }
                finally
                {
                    _Semaphore.Release();
                }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}