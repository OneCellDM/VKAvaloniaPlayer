using System.Collections.ObjectModel;
using ManagedBass;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DynamicData;
using System.ComponentModel.DataAnnotations;

namespace VKAvaloniaPlayer.ViewModels;

public class Equalizer:ReactiveObject
{
    public  string Title
    {
        get => hz + " гц";
    }
    public  int hz { get; set; }

    [Reactive]
    public  int Value { get; set; }

    public Equalizer(int hz = 0)
    {
        this.hz = hz;
    }
    
}

public class EqualizerPresset
{
    public  string Title { get; set; }
    public bool IsDefault { get; set; }
    public  List<Equalizer> Equalizers { get; set; }
}

public class SavedEqualizerData:ReactiveObject
{
    [Reactive] public ObservableCollection<EqualizerPresset> EqualizerPressets { get; set; }
    [Reactive] public int SelectedPresset { get; set; }

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

public class EqualizerViewModel:ReactiveObject
{

    private int[] channels;
    
    private List<IDisposable?> Disposibles;
    
    [Reactive]
    public  bool EqualizerManagerIsVisible { get; set; }

    [Reactive]
    public string EqualizerTitle { get; set; }
    
    [Reactive]
    public bool IsUseEqualizer { get; set; }

    [Reactive]
    public bool IsEnabled { get; set; }

    [Reactive]
    public  IReactiveCommand ResetCommand { get; set; }

    public  IReactiveCommand OpenPresetManager { get; set; }
    
    [Reactive]
    public List<Equalizer> Equalizers { get; set; }
    
    [Reactive]
    public EqualizerPresetMenagerViewModel PresetMenagerViewModel { get; set; }
    
    public EqualizerViewModel()
    {
       
        PresetMenagerViewModel = new EqualizerPresetMenagerViewModel();
        PresetMenagerViewModel.ApplyPreset(-1); 
        PresetMenagerViewModel.CloseViewEvent+=PresetMenagerViewModelOnCloseViewEvent;
        
        OpenPresetManager = ReactiveCommand.Create(() =>
        {
            EqualizerManagerIsVisible = true;
        });
        
        Disposibles = new List<IDisposable?>();
        
        
        this.WhenAnyValue(x => x.IsUseEqualizer).Subscribe((val) =>
        {
            if (val is false)
            {
               DisableEqualizer();
            }
            else
            {
                UpdateFx();
            }
            
        });
    
        
        PresetMenagerViewModel.ApplyPreset(0);
        channels = new int[8];
        
        
        PresetMenagerViewModel
            .SavedEqualizerData
            .WhenAnyValue(x => x.SelectedPresset).Subscribe((x) =>
            {
                if (x > -1)
                {
                    var preset = PresetMenagerViewModel.SavedEqualizerData.EqualizerPressets[x];

                    IsEnabled = !preset.IsDefault;


                    EqualizerTitle = preset.Title;

                    for (int i = 0; i < Disposibles?.Count; )
                    {
                        Disposibles[i].Dispose();
                        Disposibles.RemoveAt(i);
                    }
                
                    Equalizers = preset.Equalizers;
                
                    for (int i = 0; i < Equalizers.Count; i++)
                    {
                      var disposible = Equalizers[i].WhenAnyValue(x => x.Value)
                            .Subscribe((val)=>
                            {
                                if(IsUseEqualizer) UpdateFx();
                            });
                      Disposibles.Add(disposible);
                    }
                
               
                
                }
            });

    }

    private void PresetMenagerViewModelOnCloseViewEvent()
    {
        EqualizerManagerIsVisible = false;
    }


    public void DisableEqualizer()
    {
        for(int i=0; i < Equalizers?.Count; i++)
        {
            ManagedBass.DirectX8.DXParamEQParameters dXParamEQParameters = new ManagedBass.DirectX8.DXParamEQParameters()
            {
                fBandwidth = 12,
                fCenter = Equalizers[i].hz,
                fGain = 0,
            };
            Bass.FXSetParameters(channels[i], dXParamEQParameters);
        }
    }
    public void ResetEqualizer()
    {
        for (int i = 0; i < Equalizers?.Count; i++)
        {
            Equalizers[i].Value = 0;
        }
    }
    public void UpdateEqualizer()
    {
        for(int i=0; i<Equalizers?.Count; i++)
        {
            channels[i] = Bass.ChannelSetFX(PlayerControlViewModel.Player.GetStreamHandler(), EffectType.DXParamEQ, 0);
        }
    }

    public void UpdateFx()
    {
        for(int i=0; i < Equalizers?.Count; i++)
        {
            ManagedBass.DirectX8.DXParamEQParameters dXParamEQParameters = new ManagedBass.DirectX8.DXParamEQParameters()
            {
                fBandwidth = 12,
                fCenter = Equalizers[i].hz,
                fGain = Equalizers[i].Value,
            };
            Bass.FXSetParameters(channels[i], dXParamEQParameters);
        }
       PresetMenagerViewModel.SavePressets();
    }
}