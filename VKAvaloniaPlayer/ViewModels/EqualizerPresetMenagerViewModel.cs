using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Reactive.Linq;

using System.Text.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VKAvaloniaPlayer.ViewModels.Interfaces;

namespace VKAvaloniaPlayer.ViewModels;

public class EqualizerPresetMenagerViewModel:ReactiveObject,ICloseView
{
    private int[] hz = new int[] {80,170,310,600,1000,3000,6000,12000 };
    
    public const string FileName = "EqualizerPressets.json";
    public const string DefaultPresetName = "Обычный";
    
    [Reactive]
    public  bool TitleInputIsVisible { get; set; }
    
    
    [Reactive]
    public  int SelectedItemIndex { get; set; }
    [Reactive]
    public InputViewModel  TitleInputViewModel { get; set; }
    
    [Reactive]
    public SavedEqualizerData SavedEqualizerData { get; private set; }
    
    public  IReactiveCommand AddPreset { get; set; }
    public  IReactiveCommand RemovePreset { get; set; }
    
    public  IReactiveCommand ApplyPresetCommand { get; set; }
    
   
    public event ICloseView.CloseViewDelegate? CloseViewEvent;
    public IReactiveCommand CloseCommand { get; set; }


    
    private bool CanRemove()
    {
        try
        {
            var elem = SavedEqualizerData.EqualizerPressets.ElementAtOrDefault(SelectedItemIndex);
            if (elem is null) return false;

            return elem.Title == DefaultPresetName;

        }
        catch
        {
            return true;
        }
    }

    public EqualizerPresetMenagerViewModel()
    {
        
        LoadingPressets();

        TitleInputViewModel = new InputViewModel()
        {
            Message = "Название пресета",
        };
        TitleInputViewModel.CloseViewEvent += TextInputViewModelOnCloseViewEvent;
        
        AddPreset = ReactiveCommand.Create(() =>
        {
            TitleInputIsVisible = true;

        });
        RemovePreset = ReactiveCommand.Create(() =>
        {

            if (CanRemove())
            {
                SavedEqualizerData?.RemovePreset(SelectedItemIndex);
                if (SavedEqualizerData?.SelectedPresset == SelectedItemIndex)
                {
                    SavedEqualizerData.SelectedPresset = 0;
                }
            }





            SavedEqualizerData.RemoveSelectedPreset();

        });
        CloseCommand = ReactiveCommand.Create(() =>
        {
            SavePressets();
            CloseViewEvent?.Invoke();
        });

        ApplyPresetCommand = ReactiveCommand.Create(() =>
        {
           ApplyPreset(SelectedItemIndex);
        });

    }

    public void ApplyPreset(int index)
    {
        SavedEqualizerData.SelectedPresset = index;
    }

  
    private void TextInputViewModelOnCloseViewEvent()
    {
        if (TitleInputViewModel.Success)
            
        {  TitleInputViewModel.Success = false;
            
            var preset = new EqualizerPresset();

           preset.Title = TitleInputViewModel.InputText;
               
           preset.Equalizers = hz.Select(x => new Equalizer(x)).ToList();
           
           SavedEqualizerData.AddPreset(preset);
            
        }
        
        TitleInputIsVisible = false;
    }
    
    public void LoadingPressets()
    {
        try
        {
            SavedEqualizerData =
                JsonSerializer.Deserialize<SavedEqualizerData>(FileName);
        }
        catch (Exception ex)
        {
            SavedEqualizerData = new SavedEqualizerData();
        }
        finally
        {
            if (SavedEqualizerData?.GetCount() == 0 ||SavedEqualizerData?.EqualizerPressets.
                Count(x => x.Title == DefaultPresetName) == 0)
            {
                EqualizerPresset presset = new EqualizerPresset()
                {
                    Title = DefaultPresetName,
                    Equalizers = hz.Select(x => new Equalizer(x)).ToList(),
                };
                SavedEqualizerData.EqualizerPressets.Insert(0,presset); 

            }

        }
    }

    public void SavePressets()
    {
        File.WriteAllText(FileName, JsonSerializer.Serialize(SavedEqualizerData));
    }

}