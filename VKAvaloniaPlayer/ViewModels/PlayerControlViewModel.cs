using ManagedBass;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;

namespace VKAvaloniaPlayer.ViewModels
{
	public class PlayerControlViewModel : ViewModelBase
	{
		public delegate void SetCollection(IEnumerable<Models.AudioModel> AudioCollection, Models.AudioModel selectedItem);

		public static event SetCollection SetPlaylistEvent;

		public static void SetPlaylist(IEnumerable<Models.AudioModel> audioCollection, Models.AudioModel selectedItem) =>
			   SetPlaylistEvent?.Invoke(audioCollection, selectedItem);

		private static bool _PlayButtonIsVisible = true;
		private static bool _PauseButtonIsVisible = false;
		private static Thread thread;
		private double _Volume = 1;
		private Models.AudioModel _CurrentAudio = new Models.AudioModel();
		private static System.Timers.Timer _Timer = new System.Timers.Timer();

		private int _Duration;
		private int _PlayPosition;

		public static IEnumerable<Models.AudioModel> PlayList;

		public int PlayPosition
		{
			get => _PlayPosition; set => this.RaiseAndSetIfChanged(ref _PlayPosition, value);
		}

		public int Duration
		{
			get => _Duration;
			set => this.RaiseAndSetIfChanged(ref _Duration, value);
		}

		public double Volume
		{
			get => _Volume;
			set
			{
				this.RaiseAndSetIfChanged(ref _Volume, value);
				Player.SetVolume(_Volume);
			}
		}

		public Models.AudioModel CurrentAudio
		{
			get => _CurrentAudio;
			set
			{
				try
				{
					if (value != null)
					{
						this.RaiseAndSetIfChanged(ref _CurrentAudio, value);

						if (thread != null)
						{
							thread.Interrupt();
							thread.Join(500);
						}
						PauseButtonVisible();

						Duration = CurrentAudio.Duration;
						PlayPosition = 0;

						_Timer.Start();

						thread = new Thread(() =>
						 {
							 if (Player.Play(_CurrentAudio))
								 Player.SetVolume(Volume);
						 });
						thread.Priority = ThreadPriority.Lowest;
						thread.IsBackground = true;
						thread.Start();
					}
				}
				catch (Exception EX) { Debug.WriteLine(EX); }
			}
		}

		public bool PlayButtonIsVisible
		{
			get => _PlayButtonIsVisible;
			set => this.RaiseAndSetIfChanged(ref _PlayButtonIsVisible, value);
		}

		public bool PauseButtonIsVisible
		{
			get => _PauseButtonIsVisible;
			set => this.RaiseAndSetIfChanged(ref _PauseButtonIsVisible, value);
		}

		public IReactiveCommand PlayCommand { get; set; }
		public IReactiveCommand PauseCommand { get; set; }
		public IReactiveCommand NextCommand { get; set; }
		public IReactiveCommand PreviousCommand { get; set; }

		private void PlayButtonVisible()
		{
			PlayButtonIsVisible = true;
			PauseButtonIsVisible = false;
		}

		private void PauseButtonVisible()
		{
			PlayButtonIsVisible = false;
			PauseButtonIsVisible = true;
		}

		public PlayerControlViewModel()
		{
			PlayCommand = ReactiveCommand.Create(() =>
			{
				if (Player.Play())
					PauseButtonVisible();
			});
			PauseCommand = ReactiveCommand.Create(() =>
			{
				if (Player.Pause())
					PlayButtonVisible();
			});
			NextCommand = ReactiveCommand.Create(() => PlayNext());

			PreviousCommand = ReactiveCommand.Create(() => PlayPrevious());

			_Timer.Interval = 1000;
			_Timer.Elapsed += _Timer_Elapsed;
			SetPlaylistEvent += PlayerControlViewModel_SetPlaylistEvent;
		}

		private void PlayerControlViewModel_SetPlaylistEvent(IEnumerable<Models.AudioModel> AudioCollection, Models.AudioModel selectedItem)
		{
			PlayList = AudioCollection;
			CurrentAudio = selectedItem;
		}

		private void _Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => PlayPosition = Player.GetPositionSeconds());
			if (PlayPosition == Duration)
			{
				PlayNext();
			}
		}

		private void PlayNext()
		{
			int index = PlayList.ToList().IndexOf(CurrentAudio);
			if (index < (PlayList.Count() - 1)) CurrentAudio = PlayList.ToList()[index + 1];
		}

		private void PlayPrevious()
		{
			int index = PlayList.ToList().IndexOf(CurrentAudio);
			if (index > 0) CurrentAudio = PlayList.ToList()[index - 1];
		}

		public static class Player
		{
			private static int _Stream = 0;

			static Player() => Bass.Init();

			public static int GetPositionSeconds() =>
				 Convert.ToInt32(Bass.ChannelBytes2Seconds(_Stream, Bass.ChannelGetPosition(_Stream)));

			public static void SetStream(Models.AudioModel audioModel)
			{
				var url = StaticObjects.VKApi.Audio.GetById(new string[] { audioModel.OwnerID + "_" + audioModel.ID }).ElementAt(0).Url.AbsoluteUri;
				_Stream = Bass.CreateStream(url, 0, BassFlags.Default, null, IntPtr.Zero);
			}

			public static bool Play(Models.AudioModel model)
			{
				try
				{
					Stop();
					SetStream(model);
					return Play();
				}
				catch (Exception EX) { return false; }
			}

			public static bool Play() => Bass.ChannelPlay(_Stream);

			public static bool Stop()
			{
				try
				{
					Bass.ChannelStop(_Stream);
					Bass.StreamFree(_Stream);
					return true;
				}
				catch (Exception EX) { return false; }
			}

			public static bool Pause()
			{
				try
				{
					return Bass.ChannelPause(_Stream);
				}
				catch (Exception Ex) { return false; }
			}

			public static void SetVolume(double volume) => Bass.ChannelSetAttribute(_Stream, ChannelAttribute.Volume, volume);
		}
	}
}