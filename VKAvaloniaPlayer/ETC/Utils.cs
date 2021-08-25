using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using VkNet;

namespace VKAvaloniaPlayer.ETC
{
    public static class Utils
    {
        public static Bitmap LoadImageFromAssets(string path)
        {
            Uri pathUri = new Uri(@"Avares://VKAvaloniaPlayer/Assets/" + path);
            return new Bitmap(AvaloniaLocator.Current.GetService<IAssetLoader>().Open(pathUri));
        }

        public static OSPlatform CheckPlatForm()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OSPlatform.Windows;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OSPlatform.Linux;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return OSPlatform.FreeBSD;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OSPlatform.OSX;

            throw new InvalidOperationException();
        }
        public static string? GetHomeDirectory()=> Environment.GetEnvironmentVariable("HOME");
    }
}