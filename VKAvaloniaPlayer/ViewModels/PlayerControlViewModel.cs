using ManagedBass;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;

namespace VKAvaloniaPlayer.ViewModels
{
    public class PlayerControlViewModel : ViewModelBase
    {
        public delegate void SetCollection(IEnumerable<AudioModel> audioCollection, AudioModel selectedItem);

        public static event SetCollection? SetPlaylistEvent;

        public static void SetPlaylist(IEnumerable<AudioModel> audioCollection, AudioModel selectedItem) =>
            SetPlaylistEvent?.Invoke(audioCollection, selectedItem);
        

        private static bool _playButtonIsVisible = true;
        private static bool _pauseButtonIsVisible;
        private static Thread? _thread;
        private double _Volume = 1;
        private AudioModel _CurrentAudio = new();

        private static readonly System.Timers.Timer Timer = new();
        private int _Duration;
        private int _PlayPosition;
        private IReactiveCommand _PlayCommand = null!;
        private IReactiveCommand _PauseCommand = null!;
        private IReactiveCommand _NextCommand = null!;
        private IReactiveCommand _PreviousCommand = null!;

        public static IEnumerable<AudioModel>? PlayList;

        public int PlayPosition
        {
            get => _PlayPosition;
            set => this.RaiseAndSetIfChanged(ref _PlayPosition, value);
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

        public AudioModel CurrentAudio
        {
            get => _CurrentAudio;
            set
            {
                _CurrentAudio = value;
                try
                {
                    this.RaiseAndSetIfChanged(ref _CurrentAudio, value);

                    if (_thread != null)
                    {
                        _thread.Interrupt();
                        _thread.Join(500);
                    }

                    PauseButtonVisible();

                    Duration = CurrentAudio.Duration;
                    PlayPosition = 0;

                    Timer.Start();

                    _thread = new Thread(() =>
                    {
                        if (Player.Play(_CurrentAudio))
                            Player.SetVolume(Volume);
                    });
                    _thread.Priority = ThreadPriority.Lowest;
                    _thread.IsBackground = true;
                    _thread.Start();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        public bool PlayButtonIsVisible
        {
            get => _playButtonIsVisible;
            set => this.RaiseAndSetIfChanged(ref _playButtonIsVisible, value);
        }

        public bool PauseButtonIsVisible
        {
            get => _pauseButtonIsVisible;
            set => this.RaiseAndSetIfChanged(ref _pauseButtonIsVisible, value);
        }

        public IReactiveCommand PlayCommand
        {
            get => _PlayCommand;
            set => _PlayCommand = value;
        }

        public IReactiveCommand PauseCommand
        {
            get => _PauseCommand;
            set => _PauseCommand = value;
        }

        public IReactiveCommand NextCommand
        {
            get => _NextCommand;
            set => _NextCommand = value;
        }

        public IReactiveCommand PreviousCommand
        {
            get => _PreviousCommand;
            set => _PreviousCommand = value;
        }

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

            Timer.Interval = 1000;
            Timer.Elapsed += _Timer_Elapsed;
            SetPlaylistEvent += PlayerControlViewModel_SetPlaylistEvent;
        }

        private void PlayerControlViewModel_SetPlaylistEvent(IEnumerable<AudioModel>? audioCollection,
            AudioModel selectedItem)
        {
            PlayList = audioCollection;
            CurrentAudio = selectedItem;
        }

        private void _Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => PlayPosition = Player.GetPositionSeconds());

            if (PlayPosition == Duration) PlayNext();
        }

        private void PlayNext()
        {
            if (PlayList != null)
            {
                var index = PlayList.ToList().IndexOf(CurrentAudio);
                if (index < PlayList.Count() - 1) CurrentAudio = PlayList.ToList()[index + 1];
            }
        }

        private void PlayPrevious()
        {
            if (PlayList != null)
            {
                var index = PlayList.ToList().IndexOf(CurrentAudio);
                if (index > 0) CurrentAudio = PlayList.ToList()[index - 1];
            }
        }

        public static class Player
        {
            private static int _stream;

            static Player()
            {
                Bass.Init();
            }

            public static int GetPositionSeconds()
            {
                return Convert.ToInt32(Bass.ChannelBytes2Seconds(_stream, Bass.ChannelGetPosition(_stream)));
            }

            public static void SetStream(AudioModel audioModel)
            {
                var url = GlobalVars.VkApi?.Audio.GetById(new [] {audioModel.OwnerID + "_" + audioModel.ID})
                    .ElementAt(0).Url.AbsoluteUri;
                _stream = Bass.CreateStream(url, 0, BassFlags.Default, null, IntPtr.Zero);
            }

            public static bool Play(AudioModel model)
            {
                try
                {
                    Stop();
                    SetStream(model);
                    return Play();
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static bool Play()
            {
                return Bass.ChannelPlay(_stream);
            }

            public static bool Stop()
            {
                try
                {
                    Bass.ChannelStop(_stream);
                    Bass.StreamFree(_stream);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static bool Pause()
            {
                try
                {
                    return Bass.ChannelPause(_stream);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static void SetVolume(double volume)
            {
                Bass.ChannelSetAttribute(_stream, ChannelAttribute.Volume, volume);
            }
        }
    }
}