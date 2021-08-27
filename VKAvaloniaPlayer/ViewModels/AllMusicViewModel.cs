using System;
using System.Threading.Tasks;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.ViewModels.Base;
using VkNet.Model.RequestParams;

namespace VKAvaloniaPlayer.ViewModels
{
	public sealed class AllMusicViewModel : DataViewModelBase
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
				
				var res = GlobalVars.VkApi?.Audio.Get(new AudioGetParams()
				{
					Count = 500,
					Offset = (uint)Offset
				});
				
				if (res != null)
				{
					DataCollection.AddRange(res);
					Task.Run(() => { DataCollection.StartLoadImages(); });
					Offset += res.Count;
					ResponseCount = res.Count;
					
					
				}

				Loading = false;
			});
		}
	}
}