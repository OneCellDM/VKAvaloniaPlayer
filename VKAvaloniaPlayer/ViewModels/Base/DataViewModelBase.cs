using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;

using VKAvaloniaPlayer.ETC;
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

    public abstract class DataViewModelBase<T> : DataViewModelBase
    {
        protected ObservableCollection<T>? _AllDataCollection;

        private IDisposable? _SearchDisposable;

        protected string _SearchText = string.Empty;

        private IDisposable ScrolledDisposible;

        [Reactive]
        private ScrollChangedEventArgs ScrolledEventArgs { get; set; }


        [Reactive]
        public bool SearchIsVisible { get; set; }

        public int ResponseCount { get; set; }

        [Reactive]
        public ObservableCollection<T>? DataCollection { get; set; }

        public int Offset { get; set; }

        [Reactive]
        public string SearchText { get; set; }

        [Reactive]
        public int SelectedIndex { get; set; }

        public virtual void StartLoad() =>
          InvokeHandler.Start(new InvokeHandlerObject(LoadData, this));

        public DataViewModelBase()
        {
            _AllDataCollection = new ObservableCollection<T>();
            DataCollection = new ObservableCollection<T>();
        }
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

        protected virtual void LoadData()
        {
        }

        public virtual void Search(string? text)
        {

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
        public void Scrolled(object sender, ScrollChangedEventArgs args) =>
            ScrolledEventArgs = args;


    }

}