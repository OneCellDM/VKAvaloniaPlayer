
using VKAvaloniaPlayer.Models;

using static VKAvaloniaPlayer.ETC.GlobalVars;

namespace VKAvaloniaPlayer.ETC
{
    public class Events
    {
        public delegate void AudioEvent(AudioModel audioModel);
       

        public static event AudioEvent? AudioAddEvent;
        public static event AudioEvent? AudioAddToAlbumEvent;
        public static event AudioEvent? AudioRemoveFromAlbumEvent;
        public static event AudioEvent? AudioRemoveEvent;
        public static event AudioEvent? AudioRepostEvent;

        public static event Api? VkApiChanged;

        public static void VkaPiChangedCall() => VkApiChanged?.Invoke();
        public static void AudioRepostEventCall(AudioModel model) =>
           AudioRepostEvent?.Invoke(model);
        
        public static void AudioAddCall(AudioModel model) =>
                AudioAddEvent?.Invoke(model);
        public static void AudioRemoveCall(AudioModel model) =>
               AudioRemoveEvent?.Invoke(model);
        public static void AudioRmoveFromAlbumEventCall(AudioModel model) =>
            AudioRemoveFromAlbumEvent?.Invoke(model);


    }
}
