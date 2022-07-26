using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace VKAvaloniaPlayer.Notify
{

    public partial class NotifyControl : UserControl, INotifyControl
    {

        private string? _NotifyTitle;
        private string? _NotifyMessage;
        private bool _NotifyShow;


        public static readonly DirectProperty<NotifyControl, string?> NotifyTitleProperty =
             AvaloniaProperty.RegisterDirect<NotifyControl, string?>(
                 nameof(NotifyTitle),
                 o => o.NotifyTitle,
                 (o, v) => o.NotifyTitle = v, string.Empty);

        public static readonly DirectProperty<NotifyControl, string?> NotifyMessageProperty =
             AvaloniaProperty.RegisterDirect<NotifyControl, string?>(
                 nameof(NotifyMessage),
                 o => o.NotifyMessage,
                 (o, v) => o.NotifyMessage = v, string.Empty);


        public static readonly StyledProperty<int> NotifyTitleSizeProperty =
          AvaloniaProperty.Register<NotifyControl, int>(nameof(NotifyTitleSize), 16);

        public static readonly StyledProperty<int> NotifyMessageSizeProperty =
         AvaloniaProperty.Register<NotifyControl, int>(nameof(NotifyMessageSize), 16);

        public static readonly StyledProperty<IBrush> NotifyMessageForegroundProperty =
         AvaloniaProperty.Register<NotifyControl, IBrush>(nameof(NotifyMessageForeground), Brushes.Black);

        public static readonly StyledProperty<IBrush> NotifyTitleForegroundProperty =
        AvaloniaProperty.Register<NotifyControl, IBrush>(nameof(NotifyTitleForeground), Brushes.Black);

        public static readonly StyledProperty<FontWeight> NotifyTitleFontWeightProperty =
        AvaloniaProperty.Register<NotifyControl, FontWeight>(nameof(NotifyTitleFontWeight), FontWeight.Bold);

        public static readonly StyledProperty<FontWeight> NotifyMessageFontWeightProperty =
      AvaloniaProperty.Register<NotifyControl, FontWeight>(nameof(NotifyMessageFontWeight), FontWeight.Normal);

        public static readonly DirectProperty<NotifyControl, bool> NotifyShowProperty =
            AvaloniaProperty.RegisterDirect<NotifyControl, bool>(
                nameof(NotifyShow),
                o => o.NotifyShow,
                (o, v) => o.NotifyShow = v, false);

        public bool NotifyShow
        {
            get => _NotifyShow;
            set => SetAndRaise(NotifyShowProperty, ref _NotifyShow, value);
        }
        public string? NotifyTitle
        {
            get => _NotifyTitle;
            set => SetAndRaise(NotifyTitleProperty, ref _NotifyTitle, value);


        }

        public string? NotifyMessage
        {
            get => _NotifyMessage;
            set
            {
                SetAndRaise(NotifyMessageProperty, ref _NotifyMessage, value);
            }
        }


        public int? NotifyTitleSize
        {
            get => GetValue(NotifyTitleSizeProperty);
            set => SetValue(NotifyTitleSizeProperty, value);
        }

        public int? NotifyMessageSize
        {
            get => GetValue(NotifyMessageSizeProperty);
            set => SetValue(NotifyMessageSizeProperty, value);

        }


        public IBrush? NotifyTitleForeground
        {
            get => GetValue(NotifyTitleForegroundProperty);
            set => SetValue(NotifyTitleForegroundProperty, value);
        }
        public IBrush? NotifyMessageForeground
        {
            get => GetValue(NotifyMessageForegroundProperty);
            set => SetValue(NotifyMessageForegroundProperty, value);
        }


        public FontWeight? NotifyTitleFontWeight
        {
            get => GetValue(NotifyTitleFontWeightProperty);
            set => SetValue(NotifyTitleFontWeightProperty, value);
        }

        public FontWeight? NotifyMessageFontWeight
        {
            get => GetValue(NotifyMessageFontWeightProperty);
            set => SetValue(NotifyMessageFontWeightProperty, value);
        }



        public NotifyControl()
        {
            InitializeComponent();
            this.IsVisible = false;
            NotifyManager.Instance.SetNotifyControl(this);
            this.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Hide()
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => IsVisible = false);
        }


        public void ShowNotify(string Title, string Message)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {

                IsVisible = true;
                NotifyTitle = Title;
                NotifyMessage = Message;


            });
        }
    }
}
