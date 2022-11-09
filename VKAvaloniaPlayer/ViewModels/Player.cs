using ManagedBass;

using System;
using System.Linq;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;

namespace VKAvaloniaPlayer.ViewModels
{
    public partial class PlayerControlViewModel
    {
        public static class Player
        {
            private static int _stream;
            private static bool isNew = false;

            static Player()
            {
                Bass.Configure(Configuration.IncludeDefaultDevice,true);
              
                Bass.Init();
                
            }

            public static int GetPositionSeconds()
            {
                return Convert.ToInt32(Bass.ChannelBytes2Seconds(_stream, Bass.ChannelGetPosition(_stream)));
            }


            public static void SetPositon(double val)
            {
                try
                {
                    Bass.ChannelSetPosition(_stream, Bass.ChannelSeconds2Bytes(_stream, val));
                }
                catch (Exception)
                {
                }
            }

            public static void Update()
            {
                Bass.ChannelUpdate(_stream, 0);
            }

            public static void SetStream(AudioModel audioModel)
            {
                
                var url = GlobalVars.VkApi?.Audio.GetById(new[] { audioModel.GetAudioIDFormatWithAccessKey() })
                    .ElementAt(0).Url.AbsoluteUri;
               
                _stream = Bass.CreateStream(url, 0,BassFlags.Default, Dw, IntPtr.Zero);    

                var err = Bass.LastError;

                if (err is Errors.OK) isNew = false;
                
                if (isNew && err == Errors.FileOpen)
                    SetStream(audioModel);
            }
            public static void Dw(IntPtr buffer,int lenght, IntPtr user)
            {
                
                Console.WriteLine();
            }

            public static bool Play(AudioModel model)
            {
                try
                {

                    Stop();
                    isNew = true;
                    SetStream(model);
                    return Play();
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static bool Play()
            {
                Bass.Start();
                return Bass.ChannelPlay(_stream);
            }


            public static bool Stop()
            {
                try
                {
                    Bass.Stop();
                    Bass.StreamFree(_stream);
                   
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static bool Pause()
            {
                try
                {
                    return Bass.ChannelPause(_stream);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static void SetVolume(double volume)
            {
                Bass.ChannelSetAttribute(_stream, ChannelAttribute.Volume, volume);
            }
        }
    }
}