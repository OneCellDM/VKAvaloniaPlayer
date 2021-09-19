using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Layout;
using ReactiveUI;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.Models.Interfaces;
using VkNet.Model;

namespace VKAvaloniaPlayer.ViewModels.Base
{
	public abstract class DataViewModelBase : ViewModelBase
	{
		private ObservableCollection<IVkModelBase>? _DataCollection;
		private double _LastHeight = 0;
		private bool _SearchIsVisible = true;
		private bool _Loading = true;
		private bool _IsError = false;

		private IDisposable? _SearchDisposable = null;
		private string _SearchText = string.Empty;
		private int _SelectedIndex = -1;

		public ObservableCollection<IVkModelBase>? _AllDataCollection;

		public IReactiveCommand? ScrollingListEventCommand { get; set; }
		public IReactiveCommand? SelectedItemCommand { get; set; }
		public IReactiveCommand? ListBoxInitializedCommand { get; set; }

		public bool IsError
		{
			get => _IsError;
			set => this.RaiseAndSetIfChanged(ref _IsError, value);
		}

		public bool SearchIsVisible
		{
			get => _SearchIsVisible;
			set => this.RaiseAndSetIfChanged(ref _SearchIsVisible, value);
		}

		public Exceptions.ExceptionViewModel ExceptionModel { get; set; }

		public static Action? LoadMusicsAction { get; set; }

		public int ResponseCount { get; set; }

		public DataViewModelBase()
		{
			LoadMusicsAction = new Action(() =>
			{
				if (ResponseCount > 0 && IsLoading is false)
				{
					InvokeHandler.Start(new InvokeHandlerObject(LoadData, this));
				}
			});
			SelectedItemCommand = ReactiveCommand.Create((PointerPressedEventArgs e) =>
			  {
				  var contentpress = (e?.Source as ContentPresenter);

				  var model = contentpress?.Content as IVkModelBase;

				  if (model != null)
				  {
					  SelectedIndex = DataCollection.IndexOf(model);
					  SelectedItem();
				  }
			  });

			_AllDataCollection = new ObservableCollection<IVkModelBase>();
			DataCollection = _AllDataCollection;
		}

		public ObservableCollection<IVkModelBase>? DataCollection
		{
			get => _DataCollection;
			set => this.RaiseAndSetIfChanged(ref _DataCollection, value);
		}

		public bool IsLoading
		{
			get => _Loading;
			set => this.RaiseAndSetIfChanged(ref _Loading, value);
		}

		public int Offset { get; set; } = 0;

		public string SearchText
		{
			get => _SearchText;
			set => this.RaiseAndSetIfChanged(ref _SearchText, value);
		}

		public int SelectedIndex
		{
			get => _SelectedIndex;
			set
			{
				this.RaiseAndSetIfChanged(ref _SelectedIndex, value);
			}
		}

		public virtual void StartLoad() => InvokeHandler.Start(new InvokeHandlerObject(LoadData, this));

		public void StartScrollChangedObservable(Action? action, Orientation orientation)
		{
			if (ScrollingListEventCommand is null)
				ScrollingListEventCommand = ReactiveCommand.Create((ScrollChangedEventArgs e) =>
				{
					double max = 0;
					double current = 0;

					if (e.Source is ScrollViewer scrollViewer)
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

						// ReSharper disable once CompareOfFloatsByEqualityOperator
						if (max == current) action?.Invoke();
					}
				});
		}

		public void StopScrollChandegObserVable()
		{
			ScrollingListEventCommand = null;
		}

		public virtual async void LoadData()
		{
		}

		public virtual void Search(string text)
		{
			if (string.IsNullOrEmpty(SearchText))
			{
				SelectedIndex = -1;
				DataCollection = _AllDataCollection;
				StartScrollChangedObservable(LoadMusicsAction, Orientation.Vertical);
			}
			else if (_AllDataCollection != null && _AllDataCollection.Count() > 0)
			{
				StopScrollChandegObserVable();
				var searchRes = _AllDataCollection.Where(x =>
						x.Title.ToLower().Contains(SearchText.ToLower()) ||
						x.Artist.ToLower().Contains(SearchText.ToLower()))
					.Distinct();
				DataCollection = new ObservableCollection<IVkModelBase>(searchRes);
			}

			Task.Run(() => { DataCollection.StartLoadImages(); });
		}

		public virtual void SelectedItem()
		{
			if (SelectedIndex > -1)
				PlayerControlViewModel.SetPlaylist(
					new ObservableCollection<AudioModel>(DataCollection.Cast<Models.AudioModel>().ToList()),
					SelectedIndex);
		}

		public virtual void StartSearchObservable()
		{
			if (_SearchDisposable is null)
				_SearchDisposable = this.WhenAnyValue(x => x.SearchText).Subscribe((text) => Search(text.ToLower()));
		}

		public virtual void StartSearchObservable(TimeSpan timeSpan)
		{
			if (_SearchDisposable is null)
				_SearchDisposable = this.WhenAnyValue(x => x.SearchText)
					.Throttle(timeSpan)
					.Subscribe((text) => Search(text.ToLower()));
		}

		public virtual void StopSearchObservable()
		{
			_SearchDisposable?.Dispose();
			_SearchDisposable = null;
		}
	}
}