using System;
using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VKAvaloniaPlayer.ViewModels.Interfaces;
using VKAvaloniaPlayer.Views;

namespace VKAvaloniaPlayer.ViewModels;

public class InputViewModel:
    ReactiveObject,
    ICloseView
{
    [Reactive]
    public  string Message { get; set; }
    [Reactive]
    public  string InputText { get; set; }
    
    public  bool Success { get; set; }
    public  IReactiveCommand OkCommand { get; set; }
    public  IReactiveCommand CancelCommand { get; set; }

    public InputViewModel()
    {
        OkCommand = ReactiveCommand.Create<object>((inputDialog) =>
        {
            Success = true;
            if (inputDialog is Window)
            {
                var dialog = inputDialog as Window;
                dialog.Close(true);
            }
          
            CloseViewEvent?.Invoke();
           
        });
        
        CancelCommand = ReactiveCommand.Create<object>((inputDialog) =>
        {
            Success = false;
            if (inputDialog is Window)
            {
                var dialog = inputDialog as Window;
                dialog.Close(false);
            }
            CloseViewEvent?.Invoke();
        });
    }

    public event ICloseView.CloseViewDelegate? CloseViewEvent;
    public IReactiveCommand CloseCommand { get; set; }
}