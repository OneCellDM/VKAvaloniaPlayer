using Avalonia.Controls.Presenters;
using Avalonia.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.Models.Interfaces;

using VkNet.Model.Attachments;

namespace VKAvaloniaPlayer.ETC
{
    public static class ExtensionsMethods
    {
        public static T? GetContent<T>(this PointerPressedEventArgs eventArgs) where T : class
        {
            return (eventArgs?.Source as ContentPresenter)?.Content as T;
        }


        public static void AddRange(this ObservableCollection<AudioModel>? DataCollection,
            IEnumerable<Audio>? audios)
        {
            if (audios != null)
            {
                foreach (var item in audios)
                    DataCollection?.Add(new AudioModel(item));
            }

        }

        public static void AddRange(this ObservableCollection<AudioAlbumModel>? DataCollection,
            IEnumerable<AudioPlaylist> audioPlayList)
        {
            if (audioPlayList != null)
            {
                foreach (var item in audioPlayList)
                    DataCollection?.Add(new AudioAlbumModel(item));
            }
        }

        public static int FindIndex<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            int index = 0;
            bool isSearched = false;
            foreach (var item in items)
            {
                if (predicate(item))
                {
                    isSearched = true;
                    break;
                }
                index++;
            }
            return isSearched ? index : -1;
        }

        public static void StartLoadImages<T>(this ObservableCollection<T>? DataCollection) where T : IVkModelBase
        {
            try
            {
                var itemCount = DataCollection?.Count;
                for (var i = 0; i < itemCount; i++)
                {
                    if (DataCollection[i] != null)
                        DataCollection[i].Image.LoadBitmapAsync();
                }
            }
            catch (Exception EX)
            {
            }
        }
        public static void StartLoadImagesAsync<T>(this ObservableCollection<T>? DataCollection) where T : IVkModelBase
        {
            Task.Run(() => StartLoadImages(DataCollection));
        }


        public static ObservableCollection<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            ObservableCollection<T> obscollection = new(collection);
            Random rand = new();
            var itercount = collection.Count();

            for (var i = 0; i < itercount; i++)
            {
                var element = obscollection[rand.Next(itercount)];
                obscollection.Remove(element);
                obscollection.Insert(rand.Next(itercount), element);
            }

            return obscollection;
        }

        public static string GetAudioIDFormatWithAccessKey(this Audio audioModel)
        {
            return $"{audioModel.OwnerId}_{audioModel.Id}_{audioModel.AccessKey}";
        }

        public static string GetAudioIDFormatNoAccessKey(this AudioModel audioModel)
        {
            return $"{audioModel.OwnerID}_{audioModel.ID}";
        }
        public static string GetAudioIDFormatWithAccessKey(this AudioModel audioModel)
        {
            return $"{audioModel.OwnerID}_{audioModel.ID}_{audioModel.AccessKey}";
        }

        public static string GetAudioIDFormatNoAccessKey(this Audio audioModel)
        {
            return $"{audioModel.OwnerId}_{audioModel.Id}";
        }
    }
}