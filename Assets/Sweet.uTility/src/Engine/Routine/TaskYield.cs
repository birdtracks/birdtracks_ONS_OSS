using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SweetEngine.Routine
{
    public class TaskYield : CustomYieldInstruction
    {
        protected Task _task;


        public override bool keepWaiting
        {
            get
            {
                return !_task.IsCompleted;
            }
        }

        public AggregateException Exception
        {
            get { return _task.Exception; }
        }


        public TaskYield(Task task)
        {
            _task = task;
        }
    }

    public class TaskYield<T> : TaskYield
    {
        public T Result
        {
            get { return ((Task<T>)_task).Result; }
        }

        public TaskYield(Task<T> task)
            : base(task)
        {
        }
    }
}