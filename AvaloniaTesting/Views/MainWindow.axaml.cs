using Avalonia.Controls;

namespace AvaloniaTesting.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Notify.NotifyManager.Instance.PopMessage(new Notify.NotifyData("hello", "message"));
            Notify.NotifyManager.Instance.PopMessage(new Notify.NotifyData("hello", "message2"));
        }
    }
}
