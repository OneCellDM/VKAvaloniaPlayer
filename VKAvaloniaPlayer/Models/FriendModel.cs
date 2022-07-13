using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VKAvaloniaPlayer.Models.Interfaces;

using VkNet.Model;

namespace VKAvaloniaPlayer.Models
{
    public class FriendModel : IVkModelBase
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public IImageBase Image { get; set; }

        public FriendModel()
        {
            Image = new ImageModel();
        }
        public FriendModel(User user) : this()
        {
            ID = user.Id;
            Title = $"{user.FirstName} {user.LastName}";

            if (user.Photo50 != null)
                Image.ImageUrl = user.Photo50.AbsolutePath;
            
        }
        
    }
}
