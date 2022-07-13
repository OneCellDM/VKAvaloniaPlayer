using Avalonia.Media.Imaging;

using System;
using System.IO;

using VKAvaloniaPlayer.ETC;

using VkNet.Enums.Filters;

namespace VKAvaloniaPlayer.Models
{
    public class SavedAccountModel
    {
        public string? Name { get; set; }
        public long? UserID { get; set; }
        public string? Token { get; set; }

        public ImageModel Image { get; set; }

        public bool Default { get; set; } = false;

        public SavedAccountModel()
        {
            Image = new ImageModel();
        }
        
        public  void LoadAvatar()
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
                    {
                        Image.ImageUrl = res[0].Photo50.AbsoluteUri;
                        Image.LoadBitmapAsync();
                    }

                });
            }
            catch (Exception ex)
            {
            }
        }
    }
}