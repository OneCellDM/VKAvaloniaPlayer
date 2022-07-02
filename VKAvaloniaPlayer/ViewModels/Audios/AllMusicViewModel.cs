using Avalonia.Layout;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.ViewModels.Base;

using VkNet.Model.RequestParams;

namespace VKAvaloniaPlayer.ViewModels.Audios
{
    public sealed class AllMusicViewModel : AudioViewModelBase
    {

        public AllMusicViewModel()

        {
            Events.AudioAddEvent += Events_AudioAddEvent;
            Events.AudioRemoveEvent += Events_AudioRemoveEvent;
            StartSearchObservable(new TimeSpan(0, 0, 0, 1, 0));
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);
            AudioListButtons.AudioAddIsVisible = false;

        }

        private void Events_AudioRemoveEvent(AudioModel audioModel)
        {
            _AllDataCollection?.Remove(audioModel);
            DataCollection = _AllDataCollection;
        }

        private void Events_AudioAddEvent(AudioModel audioModel)
        {
            _AllDataCollection?.Insert(0, audioModel);
            DataCollection = _AllDataCollection;
        }

        public override void Search(string? text)
        {
            Task.Run(() =>
            {
                try
                {

                    DataCollection = _AllDataCollection;
                    if (string.IsNullOrEmpty(text))
                    {
                        SelectedIndex = -1;
                        DataCollection = _AllDataCollection;
                        Offset = DataCollection.Count();
                        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                        StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical));
                    }
                    else
                    {
                        Offset = 0;
                        IsLoading = true;
                        StopScrollChandegObserVable();
                        _AllDataCollection = DataCollection;
                        DataCollection = new ObservableCollection<Models.Interfaces.IVkModelBase>();
                        while (true)
                        {
                            try
                            {
                                var res = GlobalVars.VkApi?.Audio.Get(new AudioGetParams
                                {
                                    Offset = Offset,
                                    Count = 500,
                                });
                                if (res != null && res.Count > 0)
                                {
                                    var searchRes = res.Where(x =>
                                    x.Title.ToLower().Contains(text.ToLower()) ||
                                    x.Artist.ToLower().Contains(text.ToLower())).Distinct();

                                    DataCollection.AddRange(searchRes);
                                    ResponseCount = res.Count;
                                    Offset += res.Count;
                                }
                                else break;
                            }
                            catch (Exception ex)
                            {
                                break;
                            }
                        }
                        Task.Run(() => { DataCollection.StartLoadImages(); });
                    }
                }
                catch (Exception ex)
                {
                    DataCollection = _AllDataCollection;
                    SearchText = "";
                }
                finally
                {
                    IsLoading = false;
                }
            });
        }







        public override void LoadData()
        {
            var res = GlobalVars.VkApi?.Audio.Get(new AudioGetParams
            {
                Count = 500,
                Offset = (uint)Offset
            });

            if (res != null)
            {
                DataCollection.AddRange(res);
                Task.Run(() => { DataCollection.StartLoadImages(); });
                Offset += res.Count;

                ResponseCount = res.Count;
            }
        }
    }
}