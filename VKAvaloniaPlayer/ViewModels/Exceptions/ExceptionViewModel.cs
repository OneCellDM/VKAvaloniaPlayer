using ReactiveUI;

using System;

using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.ViewModels.Base;

namespace VKAvaloniaPlayer.ViewModels.Exceptions
{
    public class ExceptionViewModel : ReactiveObject
    {
        public delegate void ViewExit();

        private string _ButtonMessage;

        private string _ErrorMessage;
        private int _GridColumn;
        private int _GridColumnSpan;
        private int _GridRow;
        private int _GridRowSpan;
        private bool _IsVisible;

        public ExceptionViewModel()
        {
            CallActionCommand = ReactiveCommand.Create(() =>
            {
                ViewExitEvent?.Invoke();
                InvokeHandler.Start(new InvokeHandlerObject(Action, View));
            });
        }

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
        public DataViewModelBase View { get; set; }

        public IReactiveCommand ExitCommand { get; set; }
        public IReactiveCommand CallActionCommand { get; set; }

        public static event ViewExit ViewExitEvent;
    }
}