using Avalonia.Interactivity;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;

using VkNet.Model.RequestParams;

namespace VKAvaloniaPlayer.ViewModels
{
    public class AudioListButtonsViewModel : ReactiveObject
    {
        private bool _AudioAddIsVisible;
        private bool _AudioAddToAlbumIsVisible;

        private bool _AudioDownloadIsVisible;
        private bool _AudioRemoveIsVisible;

        public AudioListButtonsViewModel()
        {
            _AudioAddIsVisible = true;
            _AudioAddToAlbumIsVisible = true;
            _AudioDownloadIsVisible = true;
            _AudioRemoveIsVisible = true;

            AudioAddCommand = ReactiveCommand.Create(async (AudioModel vkModel) =>
            {
                if (vkModel != null)
                {
                    try
                    {
                        var res = await GlobalVars.VkApi.Audio.AddAsync(vkModel.ID,
                                                                    vkModel.OwnerID,
                                                                    vkModel.AccessKey);
                        if (res > 0)
                        {
                            vkModel.ID = res;
                            vkModel.OwnerID = (long)(GlobalVars.VkApi?.UserId ?? 0);

                            Events.AudioAddCall(vkModel);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            });
            AudioAddToAlbumCommand = ReactiveCommand.Create((RoutedEventArgs e) => { });

            AudioDownloadCommand = ReactiveCommand.Create(async (AudioModel vkModel) =>
            {
                if (vkModel != null)
                {
                    if (vkModel.IsDownload)
                        return;

                    vkModel.IsDownload = true;

                    try
                    {
                        var res = await GlobalVars.VkApi.Audio.GetByIdAsync(new string[]
                            {vkModel.GetAudioIDFormatWithAccessKey()});

                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFileAsync(res.ElementAt(0).Url,
                                string.Format("{0}-{1}.mp3", vkModel.Artist, vkModel.Title));

                            webClient.DownloadFileCompleted += delegate { vkModel.IsDownload = false; };
                            webClient.DownloadProgressChanged += (object o, DownloadProgressChangedEventArgs e) =>
                                vkModel.DownloadPercent = e.ProgressPercentage;
                        }
                    }
                    catch
                    {
                        vkModel.IsDownload = false;
                    }
                }
            });

            AudioRemoveCommand = ReactiveCommand.Create(async (AudioModel vkModel) =>
            {

                if (vkModel != null)
                {
                    if (Album is null)
                    {

                        var Awaiter = await GlobalVars.VkApi.Audio.DeleteAsync(vkModel.ID, vkModel.OwnerID);
                        if (Awaiter == true)
                        {
                            Events.AudioRemoveCall(vkModel);
                        }
                    }
                    else
                    {
                        List<string> audios = new List<string>();
                        try
                        {
                            var Audiosres = await GlobalVars.VkApi.Audio.GetAsync(new AudioGetParams()
                            {
                                OwnerId = Album.OwnerID,
                                PlaylistId = Album.ID,
                                Count = 6000
                            });
                            for (int i = 0; i < Audiosres.Count; i++)
                            {
                                if (Audiosres[i].Id == vkModel.ID)
                                    continue;

                                audios.Add(Audiosres[i].GetAudioIDFormatWithAccessKey());
                            }

                            var res = GlobalVars.VkApi.Audio.EditPlaylist(Album.OwnerID, (int)Album.ID, Album.Title,
                                null, audios);

                            if (res)
                                MusicFromAlbumViewModel.AudioRemoveEventCall(vkModel);
                        }
                        catch (Exception ex)
                        {
                        }
                        finally
                        {
                            audios.Clear();
                        }
                    }
                }
            });
        }

        public AudioAlbumModel Album { get; set; } = null;

        public bool AudioDownloadIsVisible
        {
            get => _AudioDownloadIsVisible;

            set => this.RaiseAndSetIfChanged(ref _AudioDownloadIsVisible, value);
        }

        public bool AudioAddIsVisible
        {
            get => _AudioAddIsVisible;
            set => this.RaiseAndSetIfChanged(ref _AudioAddIsVisible, value);
        }

        public bool AudioRemoveIsVisible
        {
            get => _AudioRemoveIsVisible;
            set => this.RaiseAndSetIfChanged(ref _AudioRemoveIsVisible, value);
        }

        public bool AudioAddToAlbumIsVisible
        {
            get => _AudioAddToAlbumIsVisible;
            set => this.RaiseAndSetIfChanged(ref _AudioAddToAlbumIsVisible, value);
        }

        public IReactiveCommand AudioAddCommand { get; set; }
        public IReactiveCommand AudioDownloadCommand { get; set; }
        public IReactiveCommand AudioRemoveCommand { get; set; }
        public IReactiveCommand AudioAddToAlbumCommand { get; set; }
    }
}