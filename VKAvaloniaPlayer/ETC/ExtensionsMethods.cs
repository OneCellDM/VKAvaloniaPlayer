using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
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
					if(!DataCollection[i].ImageIsloaded)
						DataCollection[i].LoadBitmap();
				
			}
			catch (Exception EX)
			{
				return;
			}
		}
		
		public static ObservableCollection<T> Shuffle<T>(this IEnumerable<T> collection)
		{
				ObservableCollection<T> obscollection = new ObservableCollection<T>(collection);
				Random rand = new Random();
				int itercount = collection.Count();
				
				for (int i = 0; i < itercount; i++)
				{
						var element = obscollection.ElementAt(rand.Next(itercount));
						obscollection.Remove(element);
						obscollection.Insert(rand.Next(itercount), element);
				}
				return obscollection;
		}
	}
}