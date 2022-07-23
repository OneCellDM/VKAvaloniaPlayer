using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

using ReactiveUI.Fody.Helpers;

namespace VKAvaloniaPlayer.Views
{
    public partial class NotifyControl : UserControl
    {
        
        public static readonly DirectProperty<NotifyControl, string?> NotifyTitleProperty =
             AvaloniaProperty.RegisterDirect<NotifyControl, string?>(
                 nameof(NotifyTitle),
                 o => o.NotifyTitle,
                 (o, v) => o.NotifyTitle = v);

        public static readonly DirectProperty<NotifyControl, string?> NotifyMessageProperty =
             AvaloniaProperty.RegisterDirect<NotifyControl, string?>(
                 nameof(NotifyMessage),
                 o => o.NotifyMessage,
                 (o, v) => o.NotifyMessage = v);

        public static readonly DirectProperty<NotifyControl, IBrush?> NotifyMessageForegroundProperty =
            AvaloniaProperty.RegisterDirect<NotifyControl, IBrush?>(
                nameof(NotifyMessageForeground),
                o => o.NotifyMessageForeground,
                (o, v) => o.NotifyMessageForeground = v);

        public static readonly DirectProperty<NotifyControl, IBrush?> NotifyTitleForegroundProperty =
           AvaloniaProperty.RegisterDirect<NotifyControl, IBrush?>(
               nameof(NotifyTitleForeground),
               o => o.NotifyTitleForeground,
               (o, v) => o.NotifyTitleForeground = v);

        [Reactive]
        public string? NotifyTitle { get; set; }
        [Reactive]
        public string? NotifyMessage { get; set; }

        [Reactive]
        public IBrush? NotifyTitleForeground { get; set; }
        [Reactive]
        public IBrush? NotifyMessageForeground { get; set; }



        public NotifyControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
