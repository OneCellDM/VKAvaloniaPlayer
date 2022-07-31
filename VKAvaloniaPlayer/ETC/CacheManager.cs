using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VKAvaloniaPlayer.Models.Interfaces;

namespace VKAvaloniaPlayer.ETC
{
    public static class CacheManager
    {
        public static string CachePath { get; set; } = "Cache";
        static CacheManager()
        {
            if (Directory.Exists(CachePath) == false)
                Directory.CreateDirectory(CachePath);
        }
        public static void SaveDataInCache(string url, in Byte[] imageBytes)
        {
            var path = Path.Combine(CachePath, GetHash(url));
            File.WriteAllBytes(path, imageBytes);

        }

        public static async Task<Stream?> GetImageStreamFromCache(string uri)
        {
            if (IsCached(uri))
            {
                return new MemoryStream(
                    await File.ReadAllBytesAsync(Path.Combine(CachePath, GetHash(uri))));
            }
            return null;
        }
        public static bool IsCached(string Uri)
        {
            return File.Exists(Path.Combine(CachePath, GetHash(Uri)));

        }

        public static string GetHash(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
