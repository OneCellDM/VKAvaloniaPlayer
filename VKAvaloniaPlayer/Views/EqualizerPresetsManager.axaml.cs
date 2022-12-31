using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VKAvaloniaPlayer.Views;

public partial class EqualizerPresetsManager : UserControl
{
    public EqualizerPresetsManager()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}