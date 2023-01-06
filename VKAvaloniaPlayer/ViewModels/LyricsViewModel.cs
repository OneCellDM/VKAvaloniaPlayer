using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VKAvaloniaPlayer.ETC;
using VKAvaloniaPlayer.Models;

namespace VKAvaloniaPlayer.ViewModels;

public class LyricsViewModel:ReactiveObject
{
    [Reactive]
    public  string Text { get; set; }

    [Reactive] 
    public bool IsVisible { get; set; } = false;

    private long? id = 0;

    public LyricsViewModel(long? lyricsId)
    {
        this.id = lyricsId;
    }

    public void StartLoad()
    {
        Task.Run(() =>
        {
            var res = GlobalVars.VkApi.Audio.GetLyrics((long)id);
            Text = res.Text;
        });
    }
}