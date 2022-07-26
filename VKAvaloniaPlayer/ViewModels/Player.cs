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

            static Player()
            {
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
                _stream = Bass.CreateStream(url, 0, BassFlags.Default, null, IntPtr.Zero);
            }

            public static bool Play(AudioModel model)
            {
                try
                {
                    Stop();
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
                    if (_stream != 0)
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