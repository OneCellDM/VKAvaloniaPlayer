using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VKAvaloniaPlayer.Views
{
    public partial class AlbumListControl : UserControl
    {
        public AlbumListControl ()
        {
            InitializeComponent();
        }

        private void InitializeComponent ()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
