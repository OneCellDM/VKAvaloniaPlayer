using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VKAvaloniaPlayer.Views
{
	public partial class VKLoginControl : UserControl
	{
		public VKLoginControl()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}