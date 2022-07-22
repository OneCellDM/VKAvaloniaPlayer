using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Layout;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.Models.Interfaces;

namespace VKAvaloniaPlayer.ViewModels.Base
{
    public abstract class AudioViewModelBase : DataViewModelBase<AudioModel>
{
        public static Action? LoadMusicsAction { get; set; }
        public AudioListButtonsViewModel AudioListButtons { get; set; }

 

    public AudioViewModelBase()
    {
        SearchIsVisible = true;
        AudioListButtons = new AudioListButtonsViewModel();
        LoadMusicsAction = () =>
        {
            if (string.IsNullOrEmpty(_SearchText))
                if (ResponseCount > 0 && IsLoading is false)
                    InvokeHandler.Start(new InvokeHandlerObject(LoadData, this));
        };

    }
      
        public override void SelectedItem(object sender, PointerPressedEventArgs args)
        {

            var model = args?.GetContent<AudioModel>();

            if (model != null)
            {
                SelectedIndex = DataCollection.IndexOf(model);

                if (SelectedIndex > -1)
                    PlayerControlViewModel.SetPlaylist(
                        new ObservableCollection<AudioModel>(DataCollection.Cast<AudioModel>().ToList()),
                        SelectedIndex);
            }
          
        }
        public override void Search(string? text)
        {
            try
            {

                if (string.IsNullOrEmpty(text))
                {
                    SelectedIndex = -1;
                    DataCollection = _AllDataCollection;
                    StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);

                }
                else if (_AllDataCollection != null && _AllDataCollection.Count() > 0)
                {
                    StopScrollChandegObserVable();

                    var searchRes = _AllDataCollection.Where(x =>
                            x.Title.ToLower().Contains(text.ToLower()) ||
                            x.Artist.ToLower().Contains(text.ToLower()))
                        .Distinct();
                    DataCollection = new ObservableCollection<AudioModel>(searchRes);
                }

                DataCollection.StartLoadImagesAsync();
            }
            catch (Exception ex)
            {
                DataCollection = _AllDataCollection;
                SearchText = "";
            }
        }
    }

        


      
}