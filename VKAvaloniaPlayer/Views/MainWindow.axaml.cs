using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VKAvaloniaPlayer.ETC;
using System.Windows;
using Avalonia.Controls.Primitives;

namespace VKAvaloniaPlayer.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

#if DEBUG
            this.AttachDevTools();
#endif
        }

        public static Window Instance { get; private set; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
    }
}