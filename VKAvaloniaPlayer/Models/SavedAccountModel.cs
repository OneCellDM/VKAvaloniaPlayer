using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Newtonsoft.Json.Serialization;
using VKAvaloniaPlayer.ETC;
using VkNet.Model;

namespace VKAvaloniaPlayer.Models
{
    public class SavedAccountModel : Models.Base.ImageModelBase
    {
        public string? Name { get; set; }
        public long? UserID { get; set; }
        public string? Token { get; set; }
        
        public  bool Default { get; set; }=false;
        
        public override void LoadBitmapAsync()
        {
            try
            {
                var profileInfoAwaiter = GlobalVars.VkApi.Users
                    .GetAsync(new[] {(long) UserID}, VkNet.Enums.Filters.ProfileFields.Photo50)
                    .GetAwaiter();
                
                profileInfoAwaiter.OnCompleted(async () =>
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var res = profileInfoAwaiter.GetResult();
                        if (res != null)
                            Image = new Bitmap(
                                new MemoryStream(await httpClient.GetByteArrayAsync(res[0].Photo50.AbsoluteUri)));
                    }
                });
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}