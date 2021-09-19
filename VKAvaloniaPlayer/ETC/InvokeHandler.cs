using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using VKAvaloniaPlayer.ViewModels.Base;
using VkNet.Exception;

namespace VKAvaloniaPlayer.ETC
{

    public class TaskHandlerObject
    {
        public Action Action { get; set; }
        public DataViewModelBase View { get; set; }

        public TaskHandlerObject(Action action,DataViewModelBase view)
        {
            this.Action = action;
            this.View = view;
        }
    }
    public static class TaskHandler
    { 

        public  delegate void TaskErrorResponsed(TaskHandlerObject handlerObject);
        public static event TaskErrorResponsed TaskErrorResponsedEvent;
        
        public static async void Start(TaskHandlerObject handlerObject)
        {
           Task.Run(() =>
           {
               try
               {
                   handlerObject.View.Loading = true;
                   handlerObject.Action.Invoke();
               }
               catch (Exception)
               {
                   TaskErrorResponsedEvent?.Invoke(handlerObject);
               }
               finally
               {
                   handlerObject.View.Loading = false;
               }
           });
        }
        static TaskHandler()
        {
            
        }

    }
}