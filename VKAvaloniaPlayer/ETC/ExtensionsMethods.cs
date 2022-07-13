using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using VKAvaloniaPlayer.Models;
using VKAvaloniaPlayer.Models.Interfaces;

using VkNet.Model.Attachments;
using VkNet.Utils;

namespace VKAvaloniaPlayer.ETC
{
    public static class ExtensionsMethods
    {
        public static void AddRange(this ObservableCollection<AudioModel>? DataCollection,
            IEnumerable<Audio>? audios)
        {
            try
            {
                var itemCount = audios.Count();
                for (var i = 0; i < itemCount; i++)
                    DataCollection.Add(new AudioModel(audios.ElementAt(i)));
            }
            catch (Exception EX)
            {
            }
        }

        public static void AddRange(this ObservableCollection<AudioAlbumModel>? DataCollection,
            VkCollection<AudioPlaylist> audios)
        {
          
                var itemCount = audios.Count();
                for (var i = 0; i < itemCount; i++)
                    DataCollection?.Add(new AudioAlbumModel(audios[i]));
           
        }

        public static void StartLoadImages<T>(this ObservableCollection<T>? DataCollection) where T : IVkModelBase
        {
            try
            {
                var itemCount = DataCollection?.Count;
                for (var i = 0; i < itemCount; i++)
                {
                    if (DataCollection[i] != null & !DataCollection[i].Image.ImageIsloaded)
                        DataCollection[i].Image.LoadBitmapAsync();
                }
            }
            catch (Exception EX)
            {
            }
        }
        
        public static ObservableCollection<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            ObservableCollection<T> obscollection = new (collection);
            Random rand = new ();
            var itercount = collection.Count();

            for (var i = 0; i < itercount; i++)
            {
                var element = obscollection.ElementAt(rand.Next(itercount));
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