using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKAvaloniaPlayer.ViewModels.Interfaces
{
    public interface ICloseView
    {
        public delegate void CloseViewDelegate();
        public event CloseViewDelegate CloseViewEvent;
        public IReactiveCommand CloseCommand { get; set; }
    }
}
