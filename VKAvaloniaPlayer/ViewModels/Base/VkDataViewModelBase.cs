using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.Models.Interfaces;
using VKAvaloniaPlayer.ViewModels.Exceptions;

namespace VKAvaloniaPlayer.ViewModels.Base
{
    public abstract class DataViewModelBase : ViewModelBase
    {
        [Reactive]
        public ExceptionViewModel ExceptionModel { get; set; }

        [Reactive]
        public bool IsError { get; set; }

        [Reactive]
        public bool IsLoading { get; set; }


    }
    public abstract class DataViewModelBase <T> : DataViewModelBase
    {
        protected ObservableCollection<IVkModelBase>? _AllDataCollection;

        private IDisposable? _SearchDisposable;


        protected string _SearchText = string.Empty;

        private IDisposable ScrolledDisposible;

        [Reactive]
        private ScrollChangedEventArgs ScrolledEventArgs { get; set; }

      
        [Reactive]
        public bool SearchIsVisible { get; set; }

      

        public static Action? LoadMusicsAction { get; set; }

        public int ResponseCount { get; set; }

        [Reactive]
        public ObservableCollection<IVkModelBase>? DataCollection { get; set; }


      

        public int Offset { get; set; }

        [Reactive]
        public string SearchText { get; set; }

        [Reactive]
        public int SelectedIndex { get; set; }
        public virtual void StartLoad() =>
          InvokeHandler.Start(new InvokeHandlerObject(LoadData, this));


        public void StartScrollChangedObservable(Action? action, Orientation orientation)
        {

            ScrolledDisposible =
                this.WhenAnyValue(vm => vm.ScrolledEventArgs)
                .Subscribe((e) =>
                {
                    try
                    {
                        double max = 0;
                        double current = 0;

                        if (e?.Source is ScrollViewer scrollViewer)
                        {
                            Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                if (orientation == Orientation.Vertical)
                                {

                                    max = scrollViewer.GetValue(ScrollViewer.VerticalScrollBarMaximumProperty);
                                    current = scrollViewer.GetValue(ScrollViewer.VerticalScrollBarValueProperty);
                                }
                                else
                                {
                                    max = scrollViewer.GetValue(ScrollViewer.HorizontalScrollBarMaximumProperty);
                                    current = scrollViewer.GetValue(ScrollViewer.HorizontalScrollBarValueProperty);
                                }
                                if (max > 0 && (max == current)) action?.Invoke();
                            });


                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                });

        }

        public void StopScrollChandegObserVable()
        {
            ScrolledDisposible?.Dispose();
            ScrolledDisposible = null;
        }

        public virtual async void LoadData()
        {
        }

        public virtual void Search(string? text)
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
                    DataCollection = new ObservableCollection<IVkModelBase>(searchRes);
                }

                Task.Run(() => { DataCollection.StartLoadImages(); });
            }
            catch (Exception ex)
            {
                DataCollection = _AllDataCollection;
                SearchText = "";
            }
        }

        public virtual void SelectedItem()
        {
           
        }

        public virtual void StartSearchObservable()
        {
            if (_SearchDisposable is null)
                _SearchDisposable = this.WhenAnyValue(x => x.SearchText)
                                        .WhereNotNull()
                                        .Subscribe(text => Search(text?.ToLower()));
        }

        public virtual void StartSearchObservable(TimeSpan timeSpan)
        {
            if (_SearchDisposable is null)
                _SearchDisposable = this.WhenAnyValue(x => x.SearchText)
                    .WhereNotNull()
                    .Throttle(timeSpan)
                    .Subscribe(text => Search(text.ToLower()));
        }

        public virtual void StopSearchObservable()
        {
            _SearchDisposable?.Dispose();
            _SearchDisposable = null;
        }


        public virtual void SelectedItem(object sender, PointerPressedEventArgs args)
        {
            
        }
        public virtual void Scrolled(object sender, ScrollChangedEventArgs args) =>
            ScrolledEventArgs = args;


    }


public abstract class AudioViewModelBase : DataViewModelBase<IVkModelBase>
{

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


        _AllDataCollection = new ObservableCollection<IVkModelBase>();
        DataCollection = _AllDataCollection;
    }
        public override void SelectedItem()
        {
            Console.WriteLine("Item selected");
            if (SelectedIndex > -1)
                PlayerControlViewModel.SetPlaylist(
                    new ObservableCollection<AudioModel>(DataCollection.Cast<AudioModel>().ToList()),
                    SelectedIndex);
        }
        public override void SelectedItem(object sender, PointerPressedEventArgs args)
        {
          
            var contentpress = args?.Source as ContentPresenter;

            var model = contentpress?.Content as IVkModelBase;

            if (model != null)
            {
                SelectedIndex = DataCollection.IndexOf(model);
                SelectedItem();
            }
          
        }
    }

        


      
}