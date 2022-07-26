using ReactiveUI;

namespace VKAvaloniaPlayer.ViewModels.Interfaces
{
    public interface ICloseView
    {
        public delegate void CloseViewDelegate();
        public event CloseViewDelegate CloseViewEvent;
        public IReactiveCommand CloseCommand { get; set; }
    }
}
