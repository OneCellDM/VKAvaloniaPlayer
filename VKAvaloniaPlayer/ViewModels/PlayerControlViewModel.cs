using ManagedBass;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Input;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.Models.Interfaces;
using VkNet.Model.Attachments;


namespace VKAvaloniaPlayer.ViewModels
{
    public class PlayerControlViewModel : ViewModelBase
    {
        private static Thread? _thread;
        private static bool _playButtonIsVisible = true;
        private static bool _pauseButtonIsVisible;
        private static bool _Repeat = false;
        private static bool _Shuffling = false;
        private static bool _Mute = false;
        private double _Volume = 1;
        private int _PlayPosition=0;
        private AudioModel _CurrentAudio = new();

        private static readonly System.Timers.Timer _Timer = new();
        
        public delegate void SetCollection(ObservableCollection<AudioModel> audioCollection, int selectedIndex);

        public static event SetCollection? SetPlaylistEvent;
        
        public static void SetPlaylist(ObservableCollection<AudioModel> audioCollection, int selectedIndex) =>
            SetPlaylistEvent?.Invoke(audioCollection, selectedIndex);

        public static ObservableCollection<AudioModel>? PlayList;
        private static ObservableCollection<AudioModel>? _allData;

        public  bool Repeat
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
                    PlayList = _allData.Shuffle<AudioModel>();

                }
                else
                    PlayList = _allData;

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
                        _thread.Join(500);
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

        public IReactiveCommand AudioPositionChangeCommand { get; set; }
        public  IReactiveCommand  RepeatToggleCommand { get; set; }
        public  IReactiveCommand MuteToggleCommand { get; set; }

        public IReactiveCommand ShuffleToogleCommand
        {
            get;
            set;
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
            AudioPositionChangeCommand= ReactiveCommand.Create((PointerCaptureLostEventArgs e) =>
            {

                Slider s = e.Source as Slider;
                if(s!=null)
                    Player.SetPositon(s.Value);
            });
            
            RepeatToggleCommand=ReactiveCommand.Create(() =>
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
            
            MuteToggleCommand=ReactiveCommand.Create(() =>
            {
                if (Mute)
                {
                    if(Volume==0)
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
            
            _Timer.Interval = 1000;
            _Timer.Elapsed += _Timer_Elapsed;
            SetPlaylistEvent += PlayerControlViewModel_SetPlaylistEvent;
        }

        private void PlayerControlViewModel_SetPlaylistEvent(ObservableCollection<AudioModel>? audioCollection, int _selectedindex)
        {
            PlayList = audioCollection;
            _allData = PlayList;
            CurrentAudio = audioCollection.ElementAt(_selectedindex);
        }

        private void _Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => PlayPosition = Player.GetPositionSeconds());
            if (PlayPosition == CurrentAudio.Duration&!Repeat) PlayNext();
            
            else if (PlayPosition == CurrentAudio.Duration&Repeat)
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

            public static void SetPositon(double val)
            {
                try
                {
                    Bass.ChannelSetPosition(_stream, Bass.ChannelSeconds2Bytes(_stream, val), PositionFlags.Bytes);
                }
                catch (Exception)
                {
                    return;
                }

            }

            public static void  Update()
            {
                Bass.ChannelUpdate(_stream, 0);
                
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