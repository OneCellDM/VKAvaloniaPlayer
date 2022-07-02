using Avalonia.Layout;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.ViewModels.Base;

using VkNet.Model.RequestParams;

namespace VKAvaloniaPlayer.ViewModels.Audios
{
    public sealed class MusicFromAlbumViewModel : AudioViewModelBase
    {
        public delegate void AudioRemove(AudioModel audioModel);

        public MusicFromAlbumViewModel(AudioAlbumModel audioAlbumModel)
        {
            Album = audioAlbumModel;

            StartSearchObservable(new TimeSpan(0, 0, 0, 0, 500));
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);

            AudioRemoveEvent += MusicFromAlbumViewModel_AudioRemoveEvent;

            if (Album.OwnerID == GlobalVars.VkApi.UserId && !Album.IsFollowing)
                AudioListButtons.AudioAddIsVisible = false;
            else
                AudioListButtons.AudioRemoveIsVisible = false;
            AudioListButtons.Album = Album;
        }

        private AudioAlbumModel Album { get; }

        public static event AudioRemove AudioRemoveEvent;

        public static void AudioRemoveEventCall(AudioModel audioModel)
        {
            AudioRemoveEvent?.Invoke(audioModel);
        }

        private void MusicFromAlbumViewModel_AudioRemoveEvent(AudioModel audioModel)
        {
            _AllDataCollection.Remove(audioModel);
        }

        public override void LoadData()
        {
            Task.Run(() =>
            {
                try
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

                        Task.Run(() => { DataCollection.StartLoadImages(); });
                        Offset += res.Count;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                IsLoading = false;
            });
        }
    }
}