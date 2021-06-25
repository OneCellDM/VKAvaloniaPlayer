using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using ReactiveUI;
namespace VKAvaloniaPlayer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
       
        
      private AllMusicListControlViewModel _AllMusicListControlViewModel = null;
        private AlbumListControlViewModel _AlbumListControlViewModel = null;
        
      private bool _VkLoginIsVisible = true;
      private int _MenuSelectionIndex = -1;
      private bool _AllMusicIsVisible = false;
      private bool _AlbumsIsVisible=false;

     public AlbumListControlViewModel AlbumListControlViewModel 
     {
            get => _AlbumListControlViewModel; 
            set => this.RaiseAndSetIfChanged(ref _AlbumListControlViewModel, value); 
     }
      public ViewModels.AllMusicListControlViewModel AllMusicListControlViewModel { 
            get => _AllMusicListControlViewModel; 
            set => this.RaiseAndSetIfChanged(ref _AllMusicListControlViewModel, value); 
      }
      public bool VkLoginIsVisible
      {
            get => _VkLoginIsVisible;
            set => this.RaiseAndSetIfChanged(ref _VkLoginIsVisible, value);
      }

      public int MenuSelectionIndex {
            get => _MenuSelectionIndex;
            set 
            { 
                this.RaiseAndSetIfChanged(ref _MenuSelectionIndex, value);
                OpenView(value);
            }
      }
      public bool AllMusicIsVisible { 
            get => _AllMusicIsVisible; 
            set =>this.RaiseAndSetIfChanged(ref _AllMusicIsVisible,value); 
      }
      public bool AlbumsIsVisible
        {
            get => _AlbumsIsVisible;
            set => this.RaiseAndSetIfChanged(ref _AlbumsIsVisible, value);
      }

      public  MainWindowViewModel()=>
            StaticObjects.VkApiChanged += StaticObjects_VkApiChanged;    
      

      private void StaticObjects_VkApiChanged()
      {
            
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                VkLoginIsVisible = false;
                AllMusicListControlViewModel = new ();
                AlbumListControlViewModel = new ();
            });
            StaticObjects.VkApiChanged -= StaticObjects_VkApiChanged;
      }
      public void OpenView(int MenuIndex)
      {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                AllMusicIsVisible = false;
                AlbumsIsVisible = false;

                switch ( MenuIndex )
                {
                    case 0:
                        {
                            if ( AllMusicListControlViewModel.AudioList.Count == 0 )
                                AllMusicListControlViewModel.Load();

                            AllMusicIsVisible = true;
                            break;
                        }
                    case 1:
                        {
                            if ( AlbumListControlViewModel.AudioAlbums.Count == 0 )
                                AlbumListControlViewModel.Load();

                            AlbumsIsVisible = true;
                            break;
                        }
                }
            });
            
      }
    }
}
