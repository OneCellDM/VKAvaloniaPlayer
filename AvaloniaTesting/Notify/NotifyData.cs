using System;

namespace AvaloniaTesting.Notify
{
    public class NotifyData
    {
        public string Message { get; set; }
        public string Title { get; set; }
        public TimeSpan ShowTIme { get; set; }
        public TimeSpan ShowDelayTime { get; set; }


        public NotifyData(string title = "", string message = "",
            TimeSpan? showTime = null, TimeSpan? showDelayTime = null)
        {
            this.Title = title;
            this.Message = message;
            this.ShowTIme = showTime ?? TimeSpan.FromSeconds(2);
            this.ShowDelayTime = showDelayTime ?? TimeSpan.FromSeconds(0.5);
        }
    }
}
