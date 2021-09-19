using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VKAvaloniaPlayer.Views
{
	public partial class MainWindow : Window
	{
		public static Window Instance { get; private set; }

		public MainWindow()
		{
			InitializeComponent();
			Instance = this;

#if DEBUG
			this.AttachDevTools();
#endif
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}