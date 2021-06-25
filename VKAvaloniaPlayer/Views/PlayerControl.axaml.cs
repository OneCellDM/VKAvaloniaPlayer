using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VKAvaloniaPlayer.Views
{
    public sealed partial class PlayerControl : UserControl
    {
        
        public PlayerControl ()
        {
            InitializeComponent();
        }

        private void InitializeComponent ()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
