using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

using ReactiveUI;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;

using Timer = System.Timers.Timer;

namespace VKAvaloniaPlayer.ViewModels
{
    public partial class PlayerControlViewModel : ViewModelBase
    {
        public delegate void OpenRepostWindowDelegate(AudioModel audioModel);
        public delegate void SetCollection(ObservableCollection<AudioModel> audioCollection, int selectedIndex);

        private static ObservableCollection<AudioModel>? PlayList;
        private static ObservableCollection<AudioModel>? _allData;

        private static PlayerControlViewModel _Instance;


        private AudioModel _CurrentAudio = new() { Duration = 60 };
        private bool _Mute;
        private bool _pauseButtonIsVisible;
        private bool _playButtonIsVisible = true;
        private int _PlayPosition = 20;
        private bool _Repeat;
        private bool _Shuffling;
        private Thread? _thread;

        private readonly Timer _Timer = new();
        private double _Volume = 1;


        public static PlayerControlViewModel Instance =>
            _Instance is null ? _Instance = new PlayerControlViewModel() : _Instance;

        public static event SetCollection? SetPlaylistEvent;

        public static event OpenRepostWindowDelegate? OpenRepostWindowEvent;

        public bool Repeat
        {
            get => _Repeat;
            set => this.RaiseAndSetIfChanged(ref _Repeat, value);
        }

        public bool Shuffling
        {
            get => _Shuffling;
            set
            {
                SetPlaylistEvent -= PlayerControlViewModel_SetPlaylistEvent;
                this.RaiseAndSetIfChanged(ref _Shuffling, value);

                if (_Shuffling)
                {
                    _allData = PlayList;
                    PlayList = _allData.Shuffle();
                }
                else
                {
                    PlayList = _allData;
                }

                SetPlaylist(PlayList, 0);
                SetPlaylistEvent += PlayerControlViewModel_SetPlaylistEvent;
            }
        }

        public bool Mute
        {
            get => _Mute;
            set => this.RaiseAndSetIfChanged(ref _Mute, value);
        }

        public int PlayPosition
        {
            get => _PlayPosition;
            set => this.RaiseAndSetIfChanged(ref _PlayPosition, value);
        }

        public double Volume
        {
            get => _Volume;
            set
            {
                this.RaiseAndSetIfChanged(ref _Volume, value);
                if (Volume == 0)
                    Mute = true;
                else Mute = false;
                Player.SetVolume(_Volume);
            }
        }

        public AudioModel CurrentAudio
        {
            get => _CurrentAudio;
            set
            {
                try
                {
                    this.RaiseAndSetIfChanged(ref _CurrentAudio, value);

                    if (_thread != null)
                    {
                        _thread.Interrupt();
                        _thread.Join(1000);
                    }

                    PauseButtonVisible();

                    PlayPosition = 0;

                    _Timer.Start();

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

        public IReactiveCommand PlayCommand { get; set; }
        public IReactiveCommand PauseCommand { get; set; }

        public IReactiveCommand NextCommand { get; set; }

        public IReactiveCommand PreviousCommand { get; set; }

        public IReactiveCommand RepeatToggleCommand { get; set; }
        public IReactiveCommand MuteToggleCommand { get; set; }

        public IReactiveCommand ShuffleToogleCommand { get; set; }
        public IReactiveCommand RepostCommand { get; set; }


        

        private PlayerControlViewModel()
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


            RepeatToggleCommand = ReactiveCommand.Create(() =>
            {
                if (Repeat)
                    Repeat = false;
                else Repeat = true;
            });
            ShuffleToogleCommand = ReactiveCommand.Create(() =>
            {
                if (!Shuffling)
                    Shuffling = true;
                else Shuffling = false;
            });

            MuteToggleCommand = ReactiveCommand.Create(() =>
            {
                if (Mute)
                {
                    if (Volume == 0)
                        return;
                    Mute = false;
                    Player.SetVolume(Volume);
                }
                else
                {
                    Mute = true;
                    Player.SetVolume(0);
                }
            });

            RepostCommand = ReactiveCommand.Create(() => Events.AudioRepostEventCall(CurrentAudio));
            _Timer.Interval = 1000;
            _Timer.Elapsed += _Timer_Elapsed;
            SetPlaylistEvent += PlayerControlViewModel_SetPlaylistEvent;
        }

        public void VolumeChanged(object sender, PointerCaptureLostEventArgs e)
        {
            Slider s = e.Source as Slider;
            if (s != null)
                Player.SetPositon(s.Value);
        }
        public static void SetPlaylist(ObservableCollection<AudioModel> audioCollection, int selectedIndex)
        {
            SetPlaylistEvent?.Invoke(audioCollection, selectedIndex);
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


        private void PlayerControlViewModel_SetPlaylistEvent(ObservableCollection<AudioModel>? audioCollection,
            int _selectedindex)
        {
            PlayList = audioCollection;
            _allData = PlayList;
            CurrentAudio = audioCollection.ElementAt(_selectedindex);
        }

        private void _Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() => PlayPosition = Player.GetPositionSeconds());
            if ((PlayPosition == CurrentAudio.Duration) & !Repeat)
            {
                PlayNext();
            }
            else if ((PlayPosition == CurrentAudio.Duration) & Repeat)
            {
                Player.Update();
                Player.Play();
            }
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
    }
}