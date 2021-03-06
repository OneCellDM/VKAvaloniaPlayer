using Avalonia.Layout;

using System;
using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.ViewModels.Base;

using VkNet.Exception;

namespace VKAvaloniaPlayer.ViewModels.Audios
{
    public sealed class RecomendationsViewModel : AudioViewModelBase
    {
        public RecomendationsViewModel()
        {
            StartSearchObservable(new TimeSpan(0, 0, 0, 0, 500));
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);

            AudioListButtons.AudioRemoveIsVisible = false;
        }

        protected override void LoadData()
        {
            var res = GlobalVars.VkApi?.Audio.GetRecommendations(count: 500, offset: (uint)Offset);
            if (res != null)
            {
                DataCollection.AddRange(res);

                DataCollection.StartLoadImagesAsync();
                Offset += res.Count;
                ResponseCount = res.Count;
            }
        }
    }
}