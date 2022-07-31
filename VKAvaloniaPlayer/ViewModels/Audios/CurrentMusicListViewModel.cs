using Avalonia.Controls;

using DynamicData;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.ViewModels.Base;

namespace VKAvaloniaPlayer.ViewModels.Audios
{
    public class CurrentMusicListViewModel : AudioViewModelBase
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
       

        public void ScrollToActiveItem()
        {
            if (_ListBox != null)
            {
                var audio = VKAvaloniaPlayer.ViewModels.PlayerControlViewModel.Instance?.CurrentAudio;
                if(audio != null)
                    _ListBox.ScrollIntoView(audio);
            }
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
            DataCollection = new ObservableCollection<AudioModel>();
            DataCollection.AddRange(audiocollection);
            _AllDataCollection = DataCollection;
        }
    }
}