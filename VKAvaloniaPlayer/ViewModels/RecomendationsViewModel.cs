using System.Threading.Tasks;
using Avalonia.Layout;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.ViewModels.Base;

namespace VKAvaloniaPlayer.ViewModels
{
    public sealed class RecomendationsViewModel : DataViewModelBase
    {
        public RecomendationsViewModel()
        {
            StartSearchObservable();
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);
        }

        public override void LoadData()
        {
            Task.Run(() =>
            {
                Loading = true;
                var res = GlobalVars.VkApi?.Audio.GetRecommendations(count: 500, offset: (uint) Offset);
                if (res != null)
                {
                    DataCollection.AddRange(res);
                    Offset += res.Count;
                    ResponseCount = res.Count;
                    Task.Run(() => { DataCollection.StartLoadImages(); });
                }

                Loading = false;
            });
        }
    }
}