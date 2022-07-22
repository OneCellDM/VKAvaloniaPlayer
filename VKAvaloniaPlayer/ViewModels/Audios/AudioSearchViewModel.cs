using Avalonia.Layout;

using ReactiveUI;

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.ViewModels.Base;

using VkNet.Model.RequestParams;

namespace VKAvaloniaPlayer.ViewModels.Audios
{
    public class AudioSearchViewModel : AudioViewModelBase
    {
        public AudioSearchViewModel()
        {
            IsLoading = false;

            StartSearchObservable(new TimeSpan(0, 0, 1));
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);
            AudioListButtons.AudioRemoveIsVisible = false;
        }

        public sealed override void StartSearchObservable(TimeSpan timeSpan)
        {
            this.WhenAnyValue(vm => vm.SearchText).Throttle(timeSpan).Subscribe(text =>
            {
                if (text is not null && text.Length > 0)
                {
                    
                    DataCollection?.Clear();
                    ResponseCount = 0;
                    Offset = 0;
                    StartLoad();
                }
            });
        }

        protected override void LoadData()
        {
            var res = GlobalVars.VkApi?.Audio.Search(new AudioSearchParams
            {
                Query = SearchText,
                Offset = Offset,
                Count = 300
            });
            if (res != null)
            {
                DataCollection.AddRange(res);
                ResponseCount = res.Count;

                DataCollection.StartLoadImagesAsync();
                Offset += res.Count;
            }
        }
    }
}