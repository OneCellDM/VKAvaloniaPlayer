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

        public static readonly DirectProperty<NotifyControl, int?> NotifyMessageSizeProperty =
            AvaloniaProperty.RegisterDirect<NotifyControl, int?>(
                nameof(NotifyMessageSize),
                o => o.NotifyMessageSize,
                (o, v) => o.NotifyMessageSize = v);

        public static readonly DirectProperty<NotifyControl, int?> NotifyTitleSizeProperty =
           AvaloniaProperty.RegisterDirect<NotifyControl, int?>(
               nameof(NotifyTitleSize),
               o => o.NotifyTitleSize,
               (o, v) => o.NotifyTitleSize = v);

        public static readonly DirectProperty<NotifyControl, FontWeight?> NotifyMessageFontWeightProperty =
            AvaloniaProperty.RegisterDirect<NotifyControl, FontWeight?>(
                nameof(NotifyMessageFontWeight),
                o => o.NotifyMessageFontWeight,
                (o, v) => o.NotifyMessageFontWeight = v);

        public static readonly DirectProperty<NotifyControl, FontWeight?> NotifyTitleFontWeightProperty =
           AvaloniaProperty.RegisterDirect<NotifyControl, FontWeight?>(
               nameof(NotifyTitleFontWeight),
               o => o.NotifyTitleFontWeight,
               (o, v) => o.NotifyTitleFontWeight = v);



        [Reactive]
        public string? NotifyTitle { get; set; }
        [Reactive]
        public string? NotifyMessage { get; set; }

        [Reactive]
        public int? NotifyTitleSize { get; set; } = 16;
        [Reactive]
        public int? NotifyMessageSize { get; set; } = 16;

        [Reactive]
        public IBrush? NotifyTitleForeground { get; set; } = Brushes.Black;
        [Reactive]
        public IBrush? NotifyMessageForeground { get; set; } = Brushes.Black;

        [Reactive]
        public FontWeight? NotifyTitleFontWeight { get; set; } = FontWeight.Bold;
        [Reactive]
        public FontWeight? NotifyMessageFontWeight { get; set; } 

        public NotifyControl()
        {
            InitializeComponent();
            this.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
