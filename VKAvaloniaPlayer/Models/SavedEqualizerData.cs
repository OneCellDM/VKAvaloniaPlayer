using System.Collections.ObjectModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using VKAvaloniaPlayer.ViewModels;

namespace VKAvaloniaPlayer.Models;

public class SavedEqualizerData : ReactiveObject
{
    [Reactive]
    public ObservableCollection<EqualizerPresset> EqualizerPressets { get; set; }

    [Reactive]
    public int SelectedPresset { get; set; }

    public SavedEqualizerData()
    {
        EqualizerPressets = new ObservableCollection<EqualizerPresset>();
    }
    public void AddPreset(EqualizerPresset presset)
    {
        EqualizerPressets.Add(presset);
    }


    public void RemovePreset(int index)
    {
        EqualizerPressets.RemoveAt(index);
    }

    public void RemovePreset(EqualizerPresset presset)
    {
        EqualizerPressets.Remove(presset);
    }

    public void RemoveSelectedPreset()
    {
        try
        {
            EqualizerPressets.RemoveAt(SelectedPresset);
        }
        catch (Exception ex)
        {

        }

    }

    public int GetCount()
    {
        if (EqualizerPressets is not null)
            return EqualizerPressets.Count;
        return 0;
    }
}
