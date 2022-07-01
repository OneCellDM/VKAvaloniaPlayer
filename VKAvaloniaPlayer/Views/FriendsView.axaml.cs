using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VKAvaloniaPlayer.Views
{
    public partial class FriendsView : UserControl
    {
        public FriendsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
