using System;
using System.Diagnostics;
using System.Threading.Tasks;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.ViewModels.Base;
using VkNet.Model.RequestParams;

namespace VKAvaloniaPlayer.ViewModels
{
	public sealed class MusicFromAlbumViewModel : DataViewModelBase
	{
		private Models.AudioAlbumModel Album { get; set; }

		public MusicFromAlbumViewModel(Models.AudioAlbumModel audioAlbumModel)
		{
			Album = audioAlbumModel;
			StartSearchObservable(new TimeSpan(0, 0, 0, 0, 500));
			StartScrollChangedObservable(DataViewModelBase.LoadMusicsAction, Avalonia.Layout.Orientation.Vertical);
		}

		public override void LoadData()
		{
			Task.Run(() =>
			{
				try
				{
					var res = GlobalVars.VkApi?.Audio.Get(new AudioGetParams()
					{
						Count = 500,
						Offset = (uint)Offset,
						PlaylistId = Album.ID,
					});
					if (res != null)
					{
						DataCollection.AddRange(res);
						ResponseCount = res.Count;

						Task.Run(() => { DataCollection.StartLoadImages(); });
						Offset += res.Count;
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
				IsLoading = false;
			});
		}
	}
}