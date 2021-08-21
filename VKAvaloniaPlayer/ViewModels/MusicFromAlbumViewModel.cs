using System;
using System.Diagnostics;
using System.Threading.Tasks;
using VKAvaloniaPlayer.ETC;
using VkNet.Model.RequestParams;

namespace VKAvaloniaPlayer.ViewModels
{
	public class MusicFromAlbumViewModel : DataViewModelBase
	{
		private Models.AudioAlbumModel Album { get; set; }

		public MusicFromAlbumViewModel(Models.AudioAlbumModel audioAlbumModel)
		{
			Album = audioAlbumModel;
			StartSearchObservable();
			StartScrollChangedObservable(DataViewModelBase.LoadMusicsAction, Avalonia.Layout.Orientation.Vertical);
		}

		public override void LoadData()
		{
			Task.Run(() =>
			{
				try
				{
					var Res = StaticObjects.VKApi.Audio.Get(new AudioGetParams()
					{
						Count = 500,
						Offset = (uint)Offset,
						PlaylistId = Album.ID,
					});
					DataCollection.AddRange(Res);
					ResponseCount = Res.Count;
					Task.Run(() => DataCollection.StartLoadImages());
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
				Loading = false;
			});
		}
	}
}