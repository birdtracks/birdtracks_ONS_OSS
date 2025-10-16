using System;
using System.Collections;
using System.Threading.Tasks;
using SweetEngine.Routine;
using UnityEngine;


namespace SweetEngine.Extensions
{
    public static class TaskExtensions
    {
        public static TaskYield AsYieldable(this Task task)
        {
            return new TaskYield(task);
        }


        public static TaskYield<T> AsYieldable<T>(this Task<T> task)
        {
            return new TaskYield<T>(task);
        }


        public static Task<T> AsOpTask<T>(this T asyncOp)
            where T : AsyncOperation
        {
            Action<AsyncOperation> opCallback = null;
            var promise = new TaskCompletionSource<T>();

            opCallback = op =>
            {
                op.completed -= opCallback;
                promise.SetResult((T)op);
            };

            asyncOp.completed += opCallback;
            return promise.Task;
        }

        public static Task<T> AsYieldTask<T>(this T yieldOp)
            where T : YieldInstruction
        {
            var promise = new TaskCompletionSource<T>();

            CoroutineHost.HostCoroutine(AsTaskRoutine(yieldOp, op =>
            {
                promise.SetResult((T)op);
            }));

            return promise.Task;
        }

        private static IEnumerator AsTaskRoutine(YieldInstruction yieldOp, Action<YieldInstruction> callback)
        {
            yield return yieldOp;
            callback(yieldOp);
        }

        public static Task<T> AsCustomYieldTask<T>(this T yieldOp)
            where T : CustomYieldInstruction
        {
            var promise = new TaskCompletionSource<T>();

            CoroutineHost.HostCoroutine(AsTaskRoutine(yieldOp, op =>
            {
                promise.SetResult((T)op);
            }));

            return promise.Task;
        }

        private static IEnumerator AsTaskRoutine(CustomYieldInstruction yieldOp, Action<CustomYieldInstruction> callback)
        {
            yield return yieldOp;
            callback(yieldOp);
        }
    }
}