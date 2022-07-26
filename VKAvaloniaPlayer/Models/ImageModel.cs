using Avalonia.Media.Imaging;

using Newtonsoft.Json;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models.Interfaces;

namespace VKAvaloniaPlayer.Models
{
    public class ImageModel : ReactiveObject, IImageBase
    {
        public static Semaphore _Semaphore = new(50, 50);

        public int DecodeWidth { get; set; }

        public string ImageUrl { get; set; }
        public bool ImageIsloaded { get; set; }


        [JsonIgnore]
        [Reactive]
        public Bitmap? Bitmap { get; set; }

        ~ImageModel()
        {

            if (Bitmap != null && ImageIsloaded)
                Bitmap.Dispose();

        }
        public async Task<Stream?> LoadImageStreamAsync()
        {
            return await Task.Run(async () =>
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
            });
        }

        public virtual async void LoadBitmapAsync()
        {
            if (string.IsNullOrEmpty(ImageUrl) is false && ImageIsloaded is false)
                try
                {
                    _Semaphore.WaitOne();
                    using (Stream? imageStream = await LoadImageStreamAsync())
                    {
                        if (imageStream is null)
                            return;

                        Bitmap = await Task.Run(() =>
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


    }
}