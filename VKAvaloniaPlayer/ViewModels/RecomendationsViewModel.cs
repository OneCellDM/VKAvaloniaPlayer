using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKAvaloniaPlayer.ETC;

namespace VKAvaloniaPlayer.ViewModels
{
	public class RecomendationsViewModel : DataViewModelBase
	{
		public RecomendationsViewModel()
		{
			StartSearchObservable();

			StartScrollChangedObservable(DataViewModelBase.LoadMusicsAction, Avalonia.Layout.Orientation.Vertical);
		}

		public override void LoadData()
		{
			Task.Run(() =>
			{
				Loading = true;
				var Res = StaticObjects.VKApi.Audio.GetRecommendations(count: 500, offset: (uint)Offset);
				DataCollection.AddRange(Res);
				Offset += Res.Count;
				ResponseCount = Res.Count;
				Task.Run(() => DataCollection.StartLoadImages());
				Loading = false;
			});
		}
	}
}