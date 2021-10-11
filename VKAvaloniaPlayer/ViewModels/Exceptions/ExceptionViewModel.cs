using System;
using ReactiveUI;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.ViewModels.Base;

namespace VKAvaloniaPlayer.ViewModels.Exceptions
{
	public class ExceptionViewModel : ReactiveObject
	{
		public delegate void ViewExit();

		public static event ViewExit ViewExitEvent;

		private string _ErrorMessage;
		private string _ButtonMessage;
		private bool _IsVisible;
		private int _GridRowSpan;
		private int _GridColumnSpan;
		private int _GridRow;
		private int _GridColumn;

		public string ButtonMessage
		{
			get => _ButtonMessage;
			set => this.RaiseAndSetIfChanged(ref _ButtonMessage, value);
		}

		public string ErrorMessage
		{
			get => _ErrorMessage;
			set => this.RaiseAndSetIfChanged(ref _ErrorMessage, value);
		}

		public bool IsVisible
		{
			get => _IsVisible;
			set => this.RaiseAndSetIfChanged(ref _IsVisible, value);
		}

		public int GridRowSpan
		{
			get => _GridRowSpan;
			set => this.RaiseAndSetIfChanged(ref _GridRowSpan, value);
		}

		public int GridColumnSpan
		{
			get => _GridColumnSpan;
			set => this.RaiseAndSetIfChanged(ref _GridColumnSpan, value);
		}

		public int GridRow
		{
			get => _GridRow;
			set => this.RaiseAndSetIfChanged(ref _GridRow, value);
		}

		public int GridColumn
		{
			get => _GridColumn;
			set => this.RaiseAndSetIfChanged(ref _GridColumn, value);
		}

		public Action Action { get; set; }
		public VkDataViewModelBase View { get; set; }

		public IReactiveCommand ExitCommand { get; set; }
		public IReactiveCommand CallActionCommand { get; set; }

		public ExceptionViewModel()
		{
			CallActionCommand = ReactiveCommand.Create(() =>
			{
				ViewExitEvent?.Invoke();
				InvokeHandler.Start(new InvokeHandlerObject(Action, View));
			});
		}
	}
}