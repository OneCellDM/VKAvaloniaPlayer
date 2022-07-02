using System;
using System.Threading.Tasks;

using VKAvaloniaPlayer.ViewModels.Base;

namespace VKAvaloniaPlayer.ETC
{
   
    public class InvokeHandlerObject
    {
        public InvokeHandlerObject(Action action, DataViewModelBase view)
        {
            Action = action;
            View = view;
        }

        public Action Action { get; set; }
        public DataViewModelBase View { get; set; }
    }

    public static class InvokeHandler
    {
        public delegate void TaskErrorResponsed(InvokeHandlerObject handlerObject, Exception ex);

        public static event TaskErrorResponsed TaskErrorResponsedEvent;

        public static async void Start(InvokeHandlerObject handlerObject)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (handlerObject.View != null)
                        handlerObject.View.IsLoading = true;

                    handlerObject.Action.Invoke();
                }
                catch (Exception ex)
                {
                    TaskErrorResponsedEvent?.Invoke(handlerObject, ex);
                }
                finally
                {
                    if (handlerObject.View != null)
                        handlerObject.View.IsLoading = false;
                }
            });
        }
    }
}