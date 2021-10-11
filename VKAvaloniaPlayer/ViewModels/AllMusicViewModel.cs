using System;
using System.Threading.Tasks;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.ViewModels.Base;
using VkNet.Model.RequestParams;

namespace VKAvaloniaPlayer.ViewModels
{
	public sealed class AllMusicViewModel : VkDataViewModelBase
	{
		public delegate void AudioAdd(AudioModel audioModel);

		public static event AudioAdd AudioAddEvent;

		public static event AudioAdd AudioRemoveEvent;

		public static void AudioAddEventCall(AudioModel audioModel) => AudioAddEvent?.Invoke(audioModel);

		public static void AudioRemoveEventCall(AudioModel audioModel) => AudioRemoveEvent?.Invoke(audioModel);

		public AllMusicViewModel()

		{
			StartSearchObservable(new TimeSpan(0, 0, 0, 0, 500));
			StartScrollChangedObservable(VkDataViewModelBase.LoadMusicsAction, Avalonia.Layout.Orientation.Vertical);
			AudioListButtons.AudioAddIsVisible = false;
			AudioAddEvent += AllMusicViewModel_AudioAddEvent;
			AudioRemoveEvent += AllMusicViewModel_AudioRemoveEvent;
		}

		private void AllMusicViewModel_AudioRemoveEvent(AudioModel audioModel) => _AllDataCollection?.Remove(audioModel);

		private void AllMusicViewModel_AudioAddEvent(AudioModel model) => _AllDataCollection?.Insert(0, model);

		public override void LoadData()
		{
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
		}
	}
}