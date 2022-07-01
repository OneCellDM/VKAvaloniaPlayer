using Avalonia.Media.Imaging;

using System;
using System.IO;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models.Base;

using VkNet.Enums.Filters;

namespace VKAvaloniaPlayer.Models
{
    public class SavedAccountModel : ImageModelBase
    {
        public string? Name { get; set; }
        public long? UserID { get; set; }
        public string? Token { get; set; }

        public bool Default { get; set; } = false;

        public override void LoadBitmapAsync()
        {
            try
            {
                var profileInfoAwaiter = GlobalVars.VkApi.Users
                    .GetAsync(new[] { (long)UserID }, ProfileFields.Photo50)
                    .GetAwaiter();

                profileInfoAwaiter.OnCompleted(async () =>
                {
                    var res = profileInfoAwaiter.GetResult();
                    if (res != null)
                        Image = new Bitmap(
                            new MemoryStream(await Utils.HttpClient.GetByteArrayAsync(res[0].Photo50.AbsoluteUri)));
                });
            }
            catch (Exception ex)
            {
            }
        }
    }
}