using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.ViewModels.Base;
using VkNet.Model.RequestParams;

namespace VKAvaloniaPlayer.ViewModels
{
	public class AudioSearchViewModel : DataViewModelBase
	{
		public AudioSearchViewModel()
		{
			IsLoading = false;

			StartSearchObservable(new TimeSpan(0, 0, 1));
			StartScrollChangedObservable(DataViewModelBase.LoadMusicsAction, Avalonia.Layout.Orientation.Vertical);
		}

		public sealed override void StartSearchObservable(TimeSpan timeSpan)
		{
			this.WhenAnyValue(vm => vm.SearchText).Throttle(timeSpan).Subscribe((text) =>
			{
				if (text is not null && text.Length > 0)
				{
					IsLoading = true;
					DataCollection?.Clear();
					ResponseCount = 0;
					Offset = 0;
					StartLoad();
				}
			});
		}

		public override void LoadData()
		{
			var res = GlobalVars.VkApi?.Audio.Search(new AudioSearchParams()
			{
				Query = SearchText,
				Offset = Offset,
				Count = 300
			});
			if (res != null)
			{
				DataCollection.AddRange(res);
				ResponseCount = res.Count;

				Task.Run(() => { DataCollection.StartLoadImages(); });
				Offset += res.Count;
			}
		}
	}
}