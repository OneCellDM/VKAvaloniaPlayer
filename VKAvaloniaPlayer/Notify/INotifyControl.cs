using Avalonia;
using Avalonia.Media;
using ReactiveUI.Fody.Helpers;

public interface INotifyControl 
{
        public string? NotifyTitle { get; set; }
        
        public string? NotifyMessage { get; set; }

       
        public int? NotifyTitleSize { get; set; } 
        
        public int? NotifyMessageSize { get; set; } 
        public IBrush? NotifyTitleForeground { get; set; } 

        public IBrush? NotifyMessageForeground { get; set; }
        public FontWeight? NotifyTitleFontWeight { get; set; } 

        public FontWeight? NotifyMessageFontWeight { get; set; } 
        
       
        public void Hide();
        public void ShowNotify(string Title,string Message);

}