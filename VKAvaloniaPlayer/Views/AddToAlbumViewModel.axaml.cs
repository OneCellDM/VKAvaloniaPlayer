using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VKAvaloniaPlayer.Views;

public partial class AddToAlbumViewModel : UserControl
{
    public AddToAlbumViewModel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}