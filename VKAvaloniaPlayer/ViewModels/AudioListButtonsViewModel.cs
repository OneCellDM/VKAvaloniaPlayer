using Avalonia.Interactivity;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.ViewModels.Audios;

using VkNet.Model.RequestParams;

namespace VKAvaloniaPlayer.ViewModels
{
    public class AudioListButtonsViewModel : ReactiveObject
    {
      
        public AudioListButtonsViewModel()
        {
            AudioAddIsVisible = true;
            AudioAddToAlbumIsVisible = true;
            AudioDownloadIsVisible = true;
            AudioRemoveIsVisible = true;
            AudioRepostIsVisible = true;
            AudioRepostCommand = ReactiveCommand.Create(async (AudioModel audioModel) =>
            {
                if (audioModel != null)
                    Events.AudioRepostEventCall(audioModel);
                
            });

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
                                Events.AudioRmoveFromAlbumEventCall(vkModel);
                        }
                        catch (Exception)
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
        [Reactive]
        public bool AudioDownloadIsVisible { get; set; }
       
        [Reactive]
        public bool AudioAddIsVisible { get; set; }
       
        [Reactive]
        public bool AudioRemoveIsVisible { get; set; }
      
        [Reactive]
        public bool AudioAddToAlbumIsVisible { get; set; }

        [Reactive]
        public bool AudioRepostIsVisible { get; set; }

        public IReactiveCommand AudioAddCommand { get; set; }
        public IReactiveCommand AudioDownloadCommand { get; set; }
        public IReactiveCommand AudioRemoveCommand { get; set; }
        public IReactiveCommand AudioAddToAlbumCommand { get; set; }
        public IReactiveCommand AudioRepostCommand { get; set; }
    }
}