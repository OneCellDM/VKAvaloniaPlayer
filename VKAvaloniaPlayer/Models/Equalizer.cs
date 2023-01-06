using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace VKAvaloniaPlayer.Models;

public class Equalizer : ReactiveObject
{
    public string Title
    {
        get => hz + " гц";
    }
    public int hz { get; set; }

    [Reactive]
    public int Value { get; set; }

    public Equalizer(int hz = 0)
    {
        this.hz = hz;
    }

}
