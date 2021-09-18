using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.Models.Interfaces;
using VKAvaloniaPlayer.ViewModels;
using VKAvaloniaPlayer.ViewModels.Base;

namespace VKAvaloniaPlayer
{
    public class CurrentMusicListViewModel:DataViewModelBase
    {
        public  CurrentMusicListViewModel()
        {
            StartSearchObservable();
            Loading = false;
            PlayerControlViewModel.SetPlaylistEvent += PlayerControlViewModelOnSetPlaylistEvent;
        }

        public override void SelectedItem()
        {
            PlayerControlViewModel.SetPlaylistEvent -= PlayerControlViewModelOnSetPlaylistEvent;
            base.SelectedItem();
            PlayerControlViewModel.SetPlaylistEvent += PlayerControlViewModelOnSetPlaylistEvent;
        }

        private void PlayerControlViewModelOnSetPlaylistEvent(IEnumerable<AudioModel> audiocollection, int selectedindex)
        {
                DataCollection = new ObservableCollection<IVkModelBase>();
                DataCollection.AddRange(audiocollection);
                _AllDataCollection = DataCollection;
        }
    }
}
