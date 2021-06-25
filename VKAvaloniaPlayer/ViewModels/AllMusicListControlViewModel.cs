using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using VkNet.Utils;

namespace VKAvaloniaPlayer.ViewModels
{
    public  class AllMusicListControlViewModel:ViewModelBase
    {
     
     public  ObservableCollection<Models.AudioModel> AudioList { get; set; }

        private  bool Isloading;
        private Models.AudioModel selectedmodel;
        public Models.AudioModel SelectedModel {
            get => selectedmodel;
            set{
                this.RaiseAndSetIfChanged(ref selectedmodel, value);
                PlayerControlViewModel.SetPlaylist(AudioList,selectedmodel);
            } 
        }
        public AllMusicListControlViewModel()
        {
            AudioList = new ObservableCollection<Models.AudioModel>();      
        }
        public void Load()
        {
            if ( Isloading == false )
            Task.Run(() =>
            {
                Isloading = true;
                
                AudioList.LoadFromVkCollection(StaticObjects.VKApi.Audio.Get(new() { Count = 6000, Offset = 0 }));
                Isloading = false;
             
            });
        }
       
    }
}
