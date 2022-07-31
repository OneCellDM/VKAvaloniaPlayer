using Avalonia.Media.Imaging;

using Newtonsoft.Json;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models.Interfaces;

namespace VKAvaloniaPlayer.Models
{
    public class ImageModel : ReactiveObject, IImageBase
    {
        public static Semaphore _Semaphore = new(Environment.ProcessorCount, Environment.ProcessorCount);

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
        private async Task<Stream?> GetImageStreamAsync()
        {
            return await Task.Run(async () =>
            {
                try
                {
                    if (string.IsNullOrEmpty(ImageUrl))
                        return null;

                    byte[]? bytes = null;


                    bytes = await Utils.HttpClient.GetByteArrayAsync(ImageUrl);
                    CacheManager.SaveDataInCache(ImageUrl,in bytes);
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

                    using (Stream? dataStream = await CacheManager.GetImageStreamFromCache(ImageUrl)
                                            ?? await GetImageStreamAsync()) 
                    {
                        if (dataStream != null)
                        {
                            Bitmap = DecodeWidth <= 0 ? new Bitmap(dataStream) 
                                                      : Bitmap.DecodeToWidth(dataStream, DecodeWidth);
                            ImageIsloaded = true;
                        }
                        
                    }
                }
                finally
                {
                    _Semaphore.Release();
                }
        }
    }
    
}