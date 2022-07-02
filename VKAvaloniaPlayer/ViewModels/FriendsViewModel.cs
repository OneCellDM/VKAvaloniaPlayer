namespace VKAvaloniaPlayer.ViewModels
{
    internal class FriendsViewModel : ViewModels.Base.DataViewModelBase <object>
    {
        public override void LoadData()
        {
            var friends = ETC.GlobalVars.VkApi.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams()
            {

            });
           


        }
    }
}
