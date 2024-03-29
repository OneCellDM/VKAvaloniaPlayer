﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using System.Reactive.Linq;

using System.Text.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VKAvaloniaPlayer.Models;
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
    public InputViewModel  TitleInputViewModel { get; set; }
    
    [Reactive]
    public SavedEqualizerData SavedEqualizerData { get; private set; }
    
    public  IReactiveCommand AddPreset { get; set; }
    public  IReactiveCommand RemovePreset { get; set; }
    
   
    
   
    public event ICloseView.CloseViewDelegate? CloseViewEvent;
    public IReactiveCommand CloseCommand { get; set; }


    

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
        RemovePreset = ReactiveCommand.Create((EqualizerPresset P) =>
        {

                SavedEqualizerData?.RemovePreset(P);
                ApplyPreset(0);

        });
        CloseCommand = ReactiveCommand.Create(() =>
        {
            SavePressets();
            CloseViewEvent?.Invoke();
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

           TitleInputViewModel.InputText = string.Empty;
           this.SavePressets();
        }
        
        TitleInputIsVisible = false;
    }
    
    public void LoadingPressets()
    {
        try
        {
            SavedEqualizerData =
                JsonSerializer.Deserialize<SavedEqualizerData>(File.ReadAllText(FileName));
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
                    IsDefault = true,
                    Title = DefaultPresetName,
                    Equalizers = hz.Select(x => new Equalizer(x)).ToList(),
                };
                SavedEqualizerData.EqualizerPressets.Insert(0,presset); 

            }

        }
    }

    public void SavePressets()
    {
        File.WriteAllText(FileName, JsonSerializer.Serialize(SavedEqualizerData).Trim());
    }

}