using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
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
        


        public PlayerControlViewModel PlayerContext => PlayerControlViewModel.Instance;

       
        public VkLoginControlViewModel? VkLoginViewModel { get; set; }
     
        [Reactive]
        public ExceptionViewModel ExceptionViewModel { get; set; }
        [Reactive]
        public AlbumsViewModel? AlbumsViewModel { get; set; }

        [Reactive]

        public VkDataViewModelBase? CurrentDataViewModel { get; set; }

        [Reactive]
        public SavedAccountModel CurrentAccountModel { get; set; }

        [Reactive]
        public bool MenuTextIsVisible { get; set; }

        [Reactive]
        public bool AlbumsIsVisible { get; set; }

        [Reactive]
        public bool CurrentDataViewIsVisible { get; set; }

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

            ExceptionViewModel.ViewExitEvent += ExceptionViewModel_ViewExitEvent;
            GlobalVars.VkApiChanged += StaticObjects_VkApiChanged;

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
                        GridColumn = 0,
                        GridRow = 0,
                        GridColumnSpan = 2,
                        GridRowSpan = 2
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
                        GridColumn = 1,
                        GridRow = 1,
                        GridColumnSpan = 0,
                        GridRowSpan = 0
                    };
                }

                if (CurrentDataViewModel == null)
                {
                    ExceptionViewModel = handlerObject.View.ExceptionModel;
                    ExceptionIsVisible = true;
                }
                else if (CurrentDataViewModel.GetType().Name == handlerObject.Action.Target.GetType().Name)
                {
                    ExceptionViewModel = CurrentDataViewModel.ExceptionModel;
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

            this.WhenAnyValue(vm => vm.MenuSelectionIndex).Subscribe(value => OpenView(value));
        }

        public void OpenView(int menuIndex)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ExceptionIsVisible = false;
                CurrentDataViewIsVisible = true;
                CurrentDataViewModel = null;
                AlbumsIsVisible = false;

                switch (menuIndex)
                {
                    case 0:
                    {
                        CurrentDataViewModel = _CurrentMusicListViewModel;
                        break;
                    }
                    case 1:
                    {
                        if (_AllMusicListViewModel == null)
                        {
                            _AllMusicListViewModel = new AllMusicViewModel();
                            _AllMusicListViewModel.StartLoad();
                        }

                        CurrentDataViewModel = _AllMusicListViewModel;
                        break;
                    }
                    case 2:
                    {
                        if (AlbumsViewModel == null)
                        {
                                AlbumsViewModel = new AlbumsViewModel();
                                AlbumsViewModel.StartLoad();
                        }

                        CurrentDataViewIsVisible = false;
                        AlbumsIsVisible = true;
                        break;
                    }
                    case 3:
                    {
                        if (_SearchViewModel == null)
                                _SearchViewModel = new AudioSearchViewModel();
                        CurrentDataViewModel = _SearchViewModel;
                        break;
                    }
                    case 4:
                    {
                        if (_RecomendationsViewModel is null)
                        {
                            _RecomendationsViewModel = new RecomendationsViewModel();
                            _RecomendationsViewModel.StartLoad();
                        }

                        CurrentDataViewModel = _RecomendationsViewModel;
                        break;
                    }
                    case 5:
                    {
                        AccountExit();
                        break;
                    }
                }

                if (CurrentDataViewModel != null)
                    if (CurrentDataViewModel.IsError)
                    {
                        ExceptionIsVisible = true;
                        ExceptionViewModel = CurrentDataViewModel.ExceptionModel;
                    }
            });
        }

        private void StaticObjects_VkApiChanged()
        {
            Console.WriteLine("VKapIChanged");
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                _CurrentMusicListViewModel = new CurrentMusicListViewModel();
                VkLoginIsVisible = false;
                CurrentAccountModel = GlobalVars.CurrentAccount;
                MenuSelectionIndex = 1;

                if (CurrentAccountModel.Image is null)
                    CurrentAccountModel.LoadBitmapAsync();
            });

            GlobalVars.VkApiChanged -= StaticObjects_VkApiChanged;
        }

        private void AccountExit()
        {
            try
            {
                Player.Stop();
                PlayerControlViewModel.Instance.CurrentAudio = null;
                
            }
            catch (Exception EX)
            {
            }

            CurrentDataViewIsVisible = false;
            AlbumsIsVisible = false;
            VkLoginIsVisible = true;

            AlbumsViewModel?.DataCollection?.Clear();
            _RecomendationsViewModel?.DataCollection?.Clear();
            _AllMusicListViewModel?.DataCollection?.Clear();
            _SearchViewModel?.DataCollection?.Clear();

            CurrentDataViewModel = null;
            AlbumsViewModel = null;
            _RecomendationsViewModel = null;
            _AllMusicListViewModel = null;
            _SearchViewModel = null;
            CurrentAccountModel = null;

            GC.Collect(0, GCCollectionMode.Optimized);
            GC.Collect(1, GCCollectionMode.Optimized);
            GC.Collect(2, GCCollectionMode.Optimized);
            GC.Collect(3, GCCollectionMode.Optimized);

            GlobalVars.VkApiChanged += StaticObjects_VkApiChanged;
        }

        private void ExceptionViewModel_ViewExitEvent()
        {
            CurrentDataViewModel.IsLoading = true;
            CurrentDataViewModel.IsError = false;
            ExceptionIsVisible = false;
        }
    }
}