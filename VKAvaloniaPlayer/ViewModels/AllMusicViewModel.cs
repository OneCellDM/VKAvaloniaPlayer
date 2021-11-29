using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Layout;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.ViewModels.Base;
using VkNet.Model.RequestParams;

namespace VKAvaloniaPlayer.ViewModels
{
    public sealed class AllMusicViewModel : VkDataViewModelBase
    {
        public delegate void AudioAdd(AudioModel audioModel);

        public AllMusicViewModel()

        {
            StartSearchObservable(new TimeSpan(0, 0, 0, 1, 0));
            StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);
            AudioListButtons.AudioAddIsVisible = false;
            AudioAddEvent += AllMusicViewModel_AudioAddEvent;
            AudioRemoveEvent += AllMusicViewModel_AudioRemoveEvent;
        }

        public override void Search(string? text)
		{
            try
            {
                DataCollection = _AllDataCollection;
                if (string.IsNullOrEmpty(text))
                {                
                    SelectedIndex = -1;
                    DataCollection = _AllDataCollection;
                    Offset = DataCollection.Count();
                    StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);
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
                        var res = GlobalVars.VkApi?.Audio.Get(new AudioGetParams
                        {
                            Offset = Offset,
                            Count = 500
                        });
                        if (res != null && res.Count > 0)
                        {
                            var searchRes=res.Where(x =>
                            x.Title.ToLower().Contains(text.ToLower()) ||
                            x.Artist.ToLower().Contains(text.ToLower())) .Distinct();

                            DataCollection.AddRange(searchRes);
                            ResponseCount = res.Count;
                            Offset += res.Count;
                        }
                        else break;
                    }
                    Task.Run(() => { DataCollection.StartLoadImages(); });
                }  
            }
            catch (Exception ex)
            {
                DataCollection = _AllDataCollection;
                SearchText = "";
            } 
            finally{
                IsLoading = false;
            }
        }
		

        public static event AudioAdd AudioAddEvent;

        public static event AudioAdd AudioRemoveEvent;

        public static void AudioAddEventCall(AudioModel audioModel)
        {
            AudioAddEvent?.Invoke(audioModel);
        }

        public static void AudioRemoveEventCall(AudioModel audioModel)
        {
            AudioRemoveEvent?.Invoke(audioModel);
        }

        private void AllMusicViewModel_AudioRemoveEvent(AudioModel audioModel)
        {
            _AllDataCollection?.Remove(audioModel);
        }

        private void AllMusicViewModel_AudioAddEvent(AudioModel model)
        {
            _AllDataCollection?.Insert(0, model);
        }

        public override void LoadData()
        {
            var res = GlobalVars.VkApi?.Audio.Get(new AudioGetParams
            {
                Count = 500,
                Offset = (uint) Offset
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