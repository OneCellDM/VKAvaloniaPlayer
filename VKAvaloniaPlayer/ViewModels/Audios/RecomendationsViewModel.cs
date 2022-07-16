﻿using Avalonia.Layout;

using System;
using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.ViewModels.Base;

using VkNet.Exception;

namespace VKAvaloniaPlayer.ViewModels.Audios
{
    public sealed class RecomendationsViewModel : AudioViewModelBase
    {
        public RecomendationsViewModel()
        {
            StartSearchObservable(new TimeSpan(0, 0, 0, 0, 500));
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);
            AudioListButtons.AudioRemoveIsVisible = false;
        }


        public override void LoadData()
        {
            Task.Run(() =>
            {
                IsLoading = true;
                try
                {
                    var res = GlobalVars.VkApi?.Audio.GetRecommendations(count: 500, offset: (uint)Offset);
                    if (res != null)
                    {
                        DataCollection.AddRange(res);

                        Task.Run(() => { DataCollection.StartLoadImages(); });
                        Offset += res.Count;
                        ResponseCount = res.Count;
                    }
                }
                catch (VkAuthorizationException exception)
                {
                }

                IsLoading = false;
            });
        }
    }
}