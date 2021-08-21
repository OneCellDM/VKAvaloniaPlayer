using System;
using System.Threading.Tasks;
using VKAvaloniaPlayer.ETC;
using VkNet.Model.RequestParams;

namespace VKAvaloniaPlayer.ViewModels
{
	public class AllMusicViewModel : DataViewModelBase
	{
		public AllMusicViewModel()
		{
			StartSearchObservable();
			StartScrollChangedObservable(DataViewModelBase.LoadMusicsAction, Avalonia.Layout.Orientation.Vertical);
		}

		public override void LoadData()
		{
			Task.Run(() =>
			{
				Loading = true;
				var Res = StaticObjects.VKApi?.Audio.Get(new AudioGetParams()
				{
					Count = 500,
					Offset = (uint)Offset
				});
				DataCollection.AddRange(Res);

				Task.Run(() => DataCollection.StartLoadImages());
				ResponseCount = Res.Count;
				Loading = false;
			});
		}
	}
}