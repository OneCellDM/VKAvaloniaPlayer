using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKAvaloniaPlayer.ViewModels
{
	internal class FriendsViewModel:ViewModels.Base.VkDataViewModelBase
	{
		public override void LoadData()
		{
			var friends = ETC.GlobalVars.VkApi.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams()
			{ 
				 
			});
			
		}
	}
}
