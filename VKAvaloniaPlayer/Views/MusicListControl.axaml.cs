using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VKAvaloniaPlayer.Views
{
	public partial class MusicListControl : UserControl
	{
		public MusicListControl()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}