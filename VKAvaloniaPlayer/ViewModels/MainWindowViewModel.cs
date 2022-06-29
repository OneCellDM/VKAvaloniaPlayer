using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;

using ReactiveUI;

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
        private const double _menuColumnWidth = 60;
        private bool _AlbumsIsVisible;
        private bool _MenuIsOpen;
        
        private AlbumsViewModel? _AlbumsViewModel;
        private AllMusicViewModel? _AllMusicViewModel;
        private AudioSearchViewModel? _AudioSearchViewModel;

        private SavedAccountModel _CurrentAccountModel;
        private bool _CurrentDataViewIsVisible;
        private VkDataViewModelBase? _CurrentDataViewModel;
        private CurrentMusicListViewModel? _CurrentMusicListViewModel;

        private bool _ExceptionIsVisible;
        private ExceptionViewModel? _ExceptionViewModel;
        private GridLength _MenuColumnWidth = new(_menuColumnWidth);
       
        private int _MenuSelectionIndex = -1;
        private bool _MenuTextIsVisible;
        private RecomendationsViewModel? _RecomendationsViewModel;
        private bool _SiderBarAnimationIsPlaying;
        private bool _VkLoginIsVisible = true;
        private VkLoginControlViewModel? _VkLoginViewModel;
        

        private bool Ismini;

      
        public MainWindowViewModel()
        {

             

           
            ExceptionViewModel.ViewExitEvent += ExceptionViewModel_ViewExitEvent;
            GlobalVars.VkApiChanged += StaticObjects_VkApiChanged;

            VkLoginViewModel = new VkLoginControlViewModel();

            OpenHideMiniPlayerCommand = ReactiveCommand.Create(() =>
            {
                if (!Ismini)
                {
                    Ismini = true;
                    MainWindow.Instance.Topmost = true;
                    MainWindow.Instance.MinHeight = 200;
                    MainWindow.Instance.Height = 200;
                    MainWindow.Instance.MaxHeight = 200;
                }
                else
                {
                    Ismini = false;
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
        }

        
        public PlayerControlViewModel PlayerContext => PlayerControlViewModel.Instance;

        public VkLoginControlViewModel? VkLoginViewModel
        {
            get => _VkLoginViewModel;
            set => this.RaiseAndSetIfChanged(ref _VkLoginViewModel, value);
        }

        public ExceptionViewModel ExceptionViewModel
        {
            get => _ExceptionViewModel;
            set => this.RaiseAndSetIfChanged(ref _ExceptionViewModel, value);
        }

        public SavedAccountModel CurrentAccountModel
        {
            get => _CurrentAccountModel;
            set => this.RaiseAndSetIfChanged(ref _CurrentAccountModel, value);
        }

        public bool MenuTextIsVisible
        {
            get => _MenuTextIsVisible;
            set => this.RaiseAndSetIfChanged(ref _MenuTextIsVisible, value);
        }

        public bool AlbumsIsVisible
        {
            get => _AlbumsIsVisible;
            set => this.RaiseAndSetIfChanged(ref _AlbumsIsVisible, value);
        }

        public bool CurrentDataViewIsVisible
        {
            get => _CurrentDataViewIsVisible;
            set => this.RaiseAndSetIfChanged(ref _CurrentDataViewIsVisible, value);
        }

        public bool VkLoginIsVisible
        {
            get => _VkLoginIsVisible;
            set => this.RaiseAndSetIfChanged(ref _VkLoginIsVisible, value);
        }

        public AlbumsViewModel? AlbumsViewModel
        {
            get => _AlbumsViewModel;
            set => this.RaiseAndSetIfChanged(ref _AlbumsViewModel, value);
        }

        public VkDataViewModelBase? CurrentDataViewModel
        {
            get => _CurrentDataViewModel;
            set => this.RaiseAndSetIfChanged(ref _CurrentDataViewModel, value);
        }

        public int MenuSelectionIndex
        {
            get => _MenuSelectionIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _MenuSelectionIndex, value);
                OpenView(value);
            }
        }

        public GridLength MenuColumnWidth
        {
            get => _MenuColumnWidth;
            set => this.RaiseAndSetIfChanged(ref _MenuColumnWidth, value);
        }

        public bool ExceptionIsVisible
        {
            get => _ExceptionIsVisible;
            set => this.RaiseAndSetIfChanged(ref _ExceptionIsVisible, value);
        }

        public IReactiveCommand AvatarPressedCommand { get; set; }
        public IReactiveCommand OpenHideMiniPlayerCommand { get; set; }

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
                        if (_AllMusicViewModel == null)
                        {
                            _AllMusicViewModel = new AllMusicViewModel();
                            _AllMusicViewModel.StartLoad();
                        }

                        CurrentDataViewModel = _AllMusicViewModel;
                        break;
                    }
                    case 2:
                    {
                        if (_AlbumsViewModel == null)
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
                        if (_AudioSearchViewModel == null) _AudioSearchViewModel = new AudioSearchViewModel();
                        CurrentDataViewModel = _AudioSearchViewModel;
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
            _AllMusicViewModel?.DataCollection?.Clear();
            _AudioSearchViewModel?.DataCollection?.Clear();
            CurrentDataViewModel = null;
            AlbumsViewModel = null;
            _RecomendationsViewModel = null;
            _AllMusicViewModel = null;
            _AudioSearchViewModel = null;
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