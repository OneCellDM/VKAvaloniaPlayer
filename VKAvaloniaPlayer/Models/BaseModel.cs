using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VKAvaloniaPlayer.Models
{
  public  enum ModelTypes{ 
        Audio,
        Album
    }
  public  class BaseModel:INotifyPropertyChanged
  {

        private static Semaphore Semaphore = new Semaphore(10, 10);
        private static HttpClient s_httpClient = new();
        private Bitmap? _cover;

        public event PropertyChangedEventHandler? PropertyChanged;
        Bitmap _Bitmap;

        public ModelTypes ModelType { get; set; }
        public long ID { get; set; }
        public long OwnerID { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string? CoverUrl { get; set; }
        public Bitmap? Cover { get => _cover; set { _cover = value; OnPropertyChanged(); } }

        private async Task<Stream> LoadImageStreamAsync()
        {    
                var data = await s_httpClient.GetByteArrayAsync(CoverUrl);
                return new MemoryStream(data);         
        }
        public async Task LoadBitmap ()
        {
            if ( !string.IsNullOrEmpty(CoverUrl) )
            {
                try
                {
                    Semaphore.WaitOne();
                    await using ( var imageStream = await LoadImageStreamAsync() )
                    {
                        Cover = await Task.Run(() => ModelType switch
                        {
                            ModelTypes.Audio =>Bitmap.DecodeToWidth(imageStream, 68, Avalonia.Visuals.Media.Imaging.BitmapInterpolationMode.LowQuality),
                            ModelTypes.Album =>Bitmap.DecodeToWidth(imageStream, 135, Avalonia.Visuals.Media.Imaging.BitmapInterpolationMode.LowQuality)                                  
                        });
                        imageStream.DisposeAsync();
    
                    }
        
                }
                finally { Semaphore.Release(); }
            }          
        }     
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

  }
}
