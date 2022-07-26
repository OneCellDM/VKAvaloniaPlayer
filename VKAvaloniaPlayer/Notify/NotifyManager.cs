using System.Collections.Generic;
using System.Threading;

namespace VKAvaloniaPlayer.Notify
{
    public class NotifyManager
    {
        private Thread Thread;

        private Queue<NotifyData> notifyDataQueUe = new Queue<NotifyData>();

        private static NotifyManager _NotifyManager;
        private INotifyControl NotifyControl { get; set; }

        public static NotifyManager Instance
        {
            get => _NotifyManager = (_NotifyManager ?? new NotifyManager());
        }
        public void SetNotifyControl(INotifyControl notifyControl) =>
                NotifyControl = notifyControl;
        private void process()
        {
            while (notifyDataQueUe.Count > 0)
            {

                var q = notifyDataQueUe.Dequeue();

                Thread.Sleep((int)q.ShowDelayTime.TotalMilliseconds);

                NotifyControl.ShowNotify(q.Title, q.Message);

                Thread.Sleep((int)q.ShowTIme.TotalMilliseconds);
                NotifyControl.Hide();


            };

        }


        public void PopMessage(NotifyData data)
        {
            notifyDataQueUe.Enqueue(data);

            if (Thread == null
                || Thread.ThreadState == ThreadState.Stopped
                || Thread.ThreadState == ThreadState.Suspended)
            {
                Thread = new Thread(process);
                Thread.IsBackground = true;
                Thread.Start();
            }


        }



    }
}