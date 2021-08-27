using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using VKAvaloniaPlayer.Models;
using VkNet.Model.Attachments;
using VKAvaloniaPlayer.Models.Interfaces;
using VkNet.Utils;

namespace VKAvaloniaPlayer.ETC
{
	public static class ExtensionsMethods
	{
		public static void AddRange(this ObservableCollection<IVkModelBase>? DataCollection, VkCollection<Audio>? audios)
		{
			try
			{
				int itemCount = audios.Count();
				for (int i = 0; i < itemCount; i++)
					DataCollection.Add(new AudioModel(audios[i]));
			}
			catch (Exception EX)
			{
				return;
			}
		}

		public static void AddRange(this ObservableCollection<IVkModelBase>? DataCollection, VkCollection<AudioPlaylist> audios)
		{
			try
			{
				int itemCount = audios.Count();
				for (int i = 0; i < itemCount; i++)
					DataCollection.Add(new AudioAlbumModel(audios[i]));
			}
			catch (Exception EX)
			{
				return;
			}
		}

		public static void StartLoadImages(this ObservableCollection<IVkModelBase>? DataCollection)
		{
			try
			{
				int itemCount = DataCollection.Count;
				for (int i = 0; i < itemCount; i++)
					DataCollection[i].LoadBitmap();
				
			}
			catch (Exception EX)
			{
				return;
			}
		}
	}
}