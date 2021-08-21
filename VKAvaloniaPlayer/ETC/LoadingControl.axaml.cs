using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VKAvaloniaPlayer.ETC
{
	public partial class LoadingControl : UserControl
	{
		public LoadingControl()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}