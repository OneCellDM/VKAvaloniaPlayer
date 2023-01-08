using Avalonia.Layout;

using System;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.ViewModels.Base;

using VkNet.Model.RequestParams;

namespace VKAvaloniaPlayer.ViewModels.Audios
{
    public sealed class MusicFromAlbumViewModel : AudioViewModelBase
    {


        public MusicFromAlbumViewModel(AudioAlbumModel audioAlbumModel)
        {
            Album = audioAlbumModel;

            StartSearchObservable(new TimeSpan(0, 0, 0, 0, 500));
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);

            Events.AudioRemoveFromAlbumEvent += MusicFromAlbumViewModel_AudioRemoveEvent;

            if (Album.OwnerID == GlobalVars.VkApi.UserId && !Album.IsFollowing)
                AudioListButtons.AudioAddIsVisible = false;
            else
                AudioListButtons.AudioRemoveIsVisible = false;
            AudioListButtons.Album = Album;
        }

        private AudioAlbumModel Album { get; }

        private void MusicFromAlbumViewModel_AudioRemoveEvent(AudioModel audioModel) =>
            _AllDataCollection?.Remove(audioModel);


        protected override void LoadData()
        {

            var res = GlobalVars.VkApi?.Audio.Get(new AudioGetParams
            {
                Count = 500,
                Offset = (uint)Offset,
                PlaylistId = Album.ID
            });
            if (res != null)
            {
                DataCollection.AddRange(res);
                ResponseCount = res.Count;

                DataCollection.StartLoadImagesAsync();
                Offset += res.Count;
                
            }
            _AllDataCollection = DataCollection;

        }

    }
}