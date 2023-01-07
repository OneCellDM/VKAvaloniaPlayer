using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using VKAvaloniaPlayer.ViewModels;

namespace VKAvaloniaPlayer.Views
{
    public class MusicListControl : UserControl
    {
        public MusicListControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LyricsScrollBorder_OnPointerLeave(object? sender, PointerEventArgs e)
        {
            if (sender is Border br)
            {
                if (br.DataContext is LyricsViewModel lr)
                {
                    lr.IsVisible = false;
                };
            }
        }
    }
}