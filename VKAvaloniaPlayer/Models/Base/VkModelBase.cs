﻿using Avalonia.Media.Imaging;
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VKAvaloniaPlayer.Models.Interfaces;

namespace VKAvaloniaPlayer.Models.Base
{
    public enum ModelTypes
    {
        Audio,
        Album
    }

    public class VkModelBase : ImageModelBase, Interfaces.IVkModelBase
    {
        private static Semaphore _semaphore = new Semaphore(5,5);
        public ModelTypes ModelType { get; set; }
        public long ID { get; set; }
        public long OwnerID { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }

        public override async void LoadBitmap()
        {
            if ((!string.IsNullOrEmpty(ImageUrl))&&!ImageIsloaded)
            {
                try
                {
                    _semaphore.WaitOne();
                    using (var imageStream = await LoadImageStreamAsync())
                    {
                        if (imageStream is null)
                        {
                            return;
                        }

                        Image = await Task.Run(() => ModelType switch
                        {
                            ModelTypes.Audio => Bitmap.DecodeToWidth(imageStream, 50,
                                Avalonia.Visuals.Media.Imaging.BitmapInterpolationMode.Default),
                            ModelTypes.Album => Bitmap.DecodeToWidth(imageStream, 135,
                                Avalonia.Visuals.Media.Imaging.BitmapInterpolationMode.Default),
                            _ => throw new System.NotImplementedException(),
                            
                            
                        });
                        ImageIsloaded=true;
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        public virtual string GetThumbUrl(VkNet.Model.AudioCover audioCover) => string.Empty;
    }
}