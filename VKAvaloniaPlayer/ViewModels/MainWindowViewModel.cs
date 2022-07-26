using Avalonia.Controls;
using Avalonia.Threading;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.ViewModels.Audios;
using VKAvaloniaPlayer.ViewModels.Base;
using VKAvaloniaPlayer.ViewModels.Exceptions;
using VKAvaloniaPlayer.Views;

using VkNet.Exception;


namespace VKAvaloniaPlayer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        private bool _IsMini;
        private bool _SiderBarAnimationIsPlaying;
        private bool _MenuIsOpen;

        private CurrentMusicListViewModel? _CurrentMusicListViewModel;
        private AllMusicViewModel? _AllMusicListViewModel;
        private AudioSearchViewModel? _SearchViewModel;
        private RecomendationsViewModel? _RecomendationsViewModel;

        public PlayerControlViewModel PlayerContext { get; set; }

        public VkLoginControlViewModel? VkLoginViewModel { get; set; }

        [Reactive]
        public ExceptionViewModel ExceptionViewModel { get; set; }

        [Reactive]
        public AlbumsViewModel? AlbumsViewModel { get; set; }

        [Reactive]
        public AudioViewModelBase? CurrentAudioViewModel { get; set; }

        [Reactive]
        public RepostViewModel? RepostViewModel { get; set; }

        [Reactive]
        public SavedAccountModel CurrentAccountModel { get; set; }

        [Reactive]
        public bool MenuTextIsVisible { get; set; }

        [Reactive]
        public bool AlbumsIsVisible { get; set; }

        [Reactive]
        public bool RepostViewIsVisible { get; set; }

        [Reactive]
        public bool CurrentAudioViewIsVisible { get; set; }

        [Reactive]

        public bool VkLoginIsVisible { get; set; } = true;
        [Reactive]

        public int MenuSelectionIndex { get; set; }

        [Reactive]
        public GridLength MenuColumnWidth { get; set; }

        [Reactive]
        public bool ExceptionIsVisible { get; set; }

        [Reactive]
        public IReactiveCommand AvatarPressedCommand { get; set; }
        [Reactive]
        public IReactiveCommand OpenHideMiniPlayerCommand { get; set; }

        public MainWindowViewModel()
        {
            PlayerContext = PlayerControlViewModel.Instance;
            Events.AudioRepostEvent += Events_AudioRepostEvent;
            ExceptionViewModel.ViewExitEvent += ExceptionViewModel_ViewExitEvent;

            Events.VkApiChanged += StaticObjects_VkApiChanged;

            VkLoginViewModel = new VkLoginControlViewModel();

            OpenHideMiniPlayerCommand = ReactiveCommand.Create(() =>
            {
                if (!_IsMini)
                {
                    _IsMini = true;
                    MainWindow.Instance.Topmost = true;
                    MainWindow.Instance.MinHeight = 200;
                    MainWindow.Instance.Height = 200;
                    MainWindow.Instance.MaxHeight = 200;
                }
                else
                {
                    _IsMini = false;
                    MainWindow.Instance.Topmost = false;
                    MainWindow.Instance.MinHeight = 0;
                    MainWindow.Instance.Height = 500;
                    MainWindow.Instance.MaxHeight = 0;
                }
            });

            InvokeHandler.TaskErrorResponsedEvent += (handlerObject, exception) =>
            {
                if (exception is UserAuthorizationFailException)
                {
                    ExceptionIsVisible = true;
                    handlerObject.View.ExceptionModel = new ExceptionViewModel
                    {
                        Action = AccountExit,
                        View = handlerObject.View,
                        ErrorMessage = "Ошибка: требуется авторизация",
                        ButtonMessage = "Открыть авторизацию",

                    };
                }
                else
                {
                    handlerObject.View.IsError = true;
                    handlerObject.View.ExceptionModel = new ExceptionViewModel
                    {
                        Action = handlerObject.Action,
                        View = handlerObject.View,
                        ErrorMessage = "Ошибка:" + exception.Message,
                        ButtonMessage = "Повторить",

                    };
                }

                if (CurrentAudioViewModel == null)
                {
                    ExceptionViewModel = handlerObject.View.ExceptionModel;
                    ExceptionIsVisible = true;
                }
                else if (CurrentAudioViewModel.GetType().Name == handlerObject.Action.Target.GetType().Name)
                {
                    ExceptionViewModel = CurrentAudioViewModel.ExceptionModel;
                    ExceptionIsVisible = true;
                }
            };

            AvatarPressedCommand = ReactiveCommand.Create(() =>
            {
                if (_SiderBarAnimationIsPlaying == false && !_MenuIsOpen)
                {
                    _SiderBarAnimationIsPlaying = true;
                    Task.Run(async () =>
                    {
                        MenuTextIsVisible = true;

                        for (var i = 60; i < 200; i += 5)
                        {
                            MenuColumnWidth = new GridLength(i);
                            await Task.Delay(new TimeSpan(0, 0, 0, 0, 1));
                        }

                        _SiderBarAnimationIsPlaying = false;
                        _MenuIsOpen = true;
                    });
                }
                else if (_SiderBarAnimationIsPlaying == false && _MenuIsOpen)
                {
                    _SiderBarAnimationIsPlaying = true;
                    Task.Run(async () =>
                    {
                        for (var i = 200; i >= 60; i -= 5)
                        {
                            MenuColumnWidth = new GridLength(i);
                            await Task.Delay(new TimeSpan(0, 0, 0, 0, 1));
                        }

                        MenuTextIsVisible = false;
                        _SiderBarAnimationIsPlaying = false;
                        _MenuIsOpen = false;
                    });
                }
            });

            this.WhenAnyValue(vm => vm.MenuSelectionIndex).Subscribe(value => OpenViewFromMenu(value));
        }

        private void Events_AudioRepostEvent(AudioModel audioModel)
        {
            RepostViewModel = new RepostViewModel(RepostToType.Friend, audioModel);
            RepostViewModel.CloseViewEvent += RepostViewModel_CloseViewEvent;
            RepostViewIsVisible = true;

        }

        private void RepostViewModel_CloseViewEvent()
        {
            RepostViewIsVisible = false;
            RepostViewModel.CloseViewEvent -= RepostViewModel_CloseViewEvent;
        }

        public void OpenViewFromMenu(int menuIndex)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ExceptionIsVisible = false;
                CurrentAudioViewIsVisible = true;
                CurrentAudioViewModel = null;
                AlbumsIsVisible = false;

                switch (menuIndex)
                {
                    case 0:
                        {
                            CurrentAudioViewModel = _CurrentMusicListViewModel;
                            break;
                        }
                    case 1:
                        {
                            if (_AllMusicListViewModel == null)
                            {
                                _AllMusicListViewModel = new AllMusicViewModel();
                                _AllMusicListViewModel.StartLoad();
                            }

                            CurrentAudioViewModel = _AllMusicListViewModel;
                            break;
                        }
                    case 2:
                        {
                            if (AlbumsViewModel == null)
                            {
                                AlbumsViewModel = new AlbumsViewModel();
                                AlbumsViewModel.StartLoad();
                            }

                            CurrentAudioViewIsVisible = false;
                            AlbumsIsVisible = true;
                            break;
                        }
                    case 3:
                        {
                            if (_SearchViewModel == null)
                                _SearchViewModel = new AudioSearchViewModel();
                            CurrentAudioViewModel = _SearchViewModel;
                            break;
                        }
                    case 4:
                        {
                            if (_RecomendationsViewModel is null)
                            {
                                _RecomendationsViewModel = new RecomendationsViewModel();
                                _RecomendationsViewModel.StartLoad();
                            }

                            CurrentAudioViewModel = _RecomendationsViewModel;
                            break;
                        }
                    case 5:
                        {
                            AccountExit();
                            break;
                        }
                }


            });
        }

        private void StaticObjects_VkApiChanged()
        {

            Dispatcher.UIThread.InvokeAsync(() =>
            {


                _CurrentMusicListViewModel = new CurrentMusicListViewModel();
                VkLoginIsVisible = false;
                CurrentAccountModel = GlobalVars.CurrentAccount;
                MenuSelectionIndex = 1;

                if (CurrentAccountModel.Image is null)
                    CurrentAccountModel.LoadAvatar();


            });

            Events.VkApiChanged -= StaticObjects_VkApiChanged;
        }

        private void AccountExit()
        {
            try
            {

                PlayerContext.CurrentAudio = null;


            }
            catch (Exception EX)
            {
            }

            CurrentAudioViewIsVisible = false;
            AlbumsIsVisible = false;
            RepostViewIsVisible = false;
            VkLoginIsVisible = true;

            AlbumsViewModel?.DataCollection?.Clear();
            _RecomendationsViewModel?.DataCollection?.Clear();
            _AllMusicListViewModel?.DataCollection?.Clear();
            _SearchViewModel?.DataCollection?.Clear();

            RepostViewModel = null;
            CurrentAudioViewModel = null;
            AlbumsViewModel = null;
            _RecomendationsViewModel = null;
            _AllMusicListViewModel = null;
            _SearchViewModel = null;
            CurrentAccountModel = null;

            GC.Collect(0, GCCollectionMode.Optimized);
            GC.Collect(1, GCCollectionMode.Optimized);
            GC.Collect(2, GCCollectionMode.Optimized);
            GC.Collect(3, GCCollectionMode.Optimized);

            Events.VkApiChanged += StaticObjects_VkApiChanged;
        }

        private void ExceptionViewModel_ViewExitEvent()
        {
            CurrentAudioViewModel.IsLoading = true;
            CurrentAudioViewModel.IsError = false;
            ExceptionIsVisible = false;
        }
    }
}