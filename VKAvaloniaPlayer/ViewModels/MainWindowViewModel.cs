using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ReactiveUI;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.Models.Interfaces;
using VKAvaloniaPlayer.ViewModels.Base;
using VkNet.Utils;


namespace VKAvaloniaPlayer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        
        private int _MenuSelectionIndex = -1;
        private const double _menuColumnWidth = 60;
        private bool _SiderBarAnimationIsPlaying;
        private bool _MenuTextIsVisible;
        private bool _AlbumsIsVisible;
        private bool _CurrentDataViewIsVisible;
        private bool _VkLoginIsVisible = true;
        private bool _MenuIsOpen = false;
        private AlbumsViewModel? _AlbumsViewModel;
        private DataViewModelBase? _CurrentDataViewModel;
        private AllMusicViewModel? _AllMusicViewModel;
        private AudioSearchViewModel? _AudioSearchViewModel;
        private RecomendationsViewModel? _RecomendationsViewModel;
        private CurrentMusicListViewModel? _CurrentMusicListViewModel;


        private SavedAccountModel _CurrentAccountModel;
        private GridLength _MenuColumnWidth = new GridLength(_menuColumnWidth);

       
        public  SavedAccountModel CurrentAccountModel
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

        public DataViewModelBase? CurrentDataViewModel
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


        public IReactiveCommand MenuPointEnterCommand { get; set; }
        public IReactiveCommand MenuPointLeaveCommand { get; set; }
     
        public void OpenView(int menuIndex)
        {
            
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
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
                            _AllMusicViewModel.LoadData();
                        }

                        CurrentDataViewModel = _AllMusicViewModel;
                        break;
                    }
                    case 2:
                    {
                        if (_AlbumsViewModel == null)
                        {
                            AlbumsViewModel = new AlbumsViewModel();
                            AlbumsViewModel.LoadData();
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
                            _RecomendationsViewModel.LoadData();
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
                
              
            });
          
        }

        private void StaticObjects_VkApiChanged()
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                _CurrentMusicListViewModel = new CurrentMusicListViewModel();
                VkLoginIsVisible = false;
                CurrentAccountModel = GlobalVars.CurrentAccount;
                MenuSelectionIndex = 1;
                
                if(CurrentAccountModel.Image is null)
                    CurrentAccountModel.LoadBitmap();
                
            });
            
            GlobalVars.VkApiChanged -= StaticObjects_VkApiChanged;
        }

        private void AccountExit()
        {
            PlayerControlViewModel.Player.Pause();
            
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
            GC.Collect(0,GCCollectionMode.Optimized);
            GC.Collect(1,GCCollectionMode.Optimized);
            GC.Collect(2,GCCollectionMode.Optimized);
            GC.Collect(3,GCCollectionMode.Optimized);
            GlobalVars.VkApiChanged += StaticObjects_VkApiChanged;

        }

        public MainWindowViewModel()
        {

            GlobalVars.VkApiChanged += StaticObjects_VkApiChanged;
            MenuPointEnterCommand = ReactiveCommand.Create((object obj) =>
            {
                
                if (_SiderBarAnimationIsPlaying==false&&!_MenuIsOpen)
                {
                    _SiderBarAnimationIsPlaying = true;
                    Task.Run(async () =>
                    {
                        MenuTextIsVisible = true;

                        for (int i = 60; i < 200; i += 2)
                        {
                            MenuColumnWidth = new GridLength(i);
                            await Task.Delay(1);
                        }

                        _SiderBarAnimationIsPlaying = false;
                        _MenuIsOpen = true;
                    });
                }
            });
            MenuPointLeaveCommand = ReactiveCommand.Create((object obj) =>
            {
                if (_SiderBarAnimationIsPlaying==false&&_MenuIsOpen)
                {
                    _SiderBarAnimationIsPlaying = true;
                    Task.Run(async () =>
                    {
                        for (int i = 200; i >= 60; i -= 2)
                        {
                            MenuColumnWidth = new GridLength(i);
                            await Task.Delay(1);
                        }
                        MenuTextIsVisible = false;
                        _SiderBarAnimationIsPlaying = false;
                        _MenuIsOpen = false;
                    });
                }
            });
        }
    }
}