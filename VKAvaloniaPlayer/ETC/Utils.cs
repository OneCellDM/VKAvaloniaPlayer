using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

using System;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace VKAvaloniaPlayer.ETC
{
    public static class Utils
    {
        public static readonly HttpClient HttpClient = new();
        public static readonly Random Random = new Random();

        public static Bitmap? LoadImageFromAssets(string path)
        {
            Uri pathUri = new(@"Avares://VKAvaloniaPlayer/Assets/" + path);

            var res = AvaloniaLocator.Current?.GetService<IAssetLoader>()?.Open(pathUri);
            return res != null ? new Bitmap(res) : null;
           
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

        
        public static string? GetHomeDirectory()
        {
            return Environment.GetEnvironmentVariable("HOME");
        }
    }
}