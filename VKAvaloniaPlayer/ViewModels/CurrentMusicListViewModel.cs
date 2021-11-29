using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.Models.Interfaces;
using VKAvaloniaPlayer.ViewModels;
using VKAvaloniaPlayer.ViewModels.Base;

namespace VKAvaloniaPlayer
{
    public class CurrentMusicListViewModel : VkDataViewModelBase
    {
        public CurrentMusicListViewModel()
        {
            SearchIsVisible = false;
            IsLoading = false;
            PlayerControlViewModel.SetPlaylistEvent += PlayerControlViewModelOnSetPlaylistEvent;
            AudioListButtons.AudioRemoveIsVisible = false;
            AudioListButtons.AudioAddIsVisible = false;
            AudioListButtons.AudioAddToAlbumIsVisible = false;
        }

        public override void SelectedItem()
        {
            PlayerControlViewModel.SetPlaylistEvent -= PlayerControlViewModelOnSetPlaylistEvent;
            base.SelectedItem();
            PlayerControlViewModel.SetPlaylistEvent += PlayerControlViewModelOnSetPlaylistEvent;
        }

        private void PlayerControlViewModelOnSetPlaylistEvent(IEnumerable<AudioModel> audiocollection,
            int selectedindex)
        {
            DataCollection = new ObservableCollection<IVkModelBase>();
            DataCollection.AddRange(audiocollection);
            _AllDataCollection = DataCollection;
        }
    }
}