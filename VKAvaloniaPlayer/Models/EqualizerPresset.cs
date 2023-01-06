using System.Collections.Generic;

using VKAvaloniaPlayer.Models;

namespace VKAvaloniaPlayer.ViewModels;

public class EqualizerPresset
{
    public  string Title { get; set; }
    public bool IsDefault { get; set; }
    public  List<Equalizer> Equalizers { get; set; }
}
