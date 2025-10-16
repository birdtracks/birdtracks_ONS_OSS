using System;
using System.Collections;
using SweetEngine.Routine;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public static class YieldInstructionExtensions
    {
        public static void Then(this YieldInstruction yieldInstruction, Action callback)
        {
            CoroutineHost.HostCoroutine(DoThen(yieldInstruction, callback));
        }

        private static IEnumerator DoThen(YieldInstruction yieldInstruction, Action callback)
        {
            if (yieldInstruction == null)
            {
                callback?.Invoke();
            }

            yield return yieldInstruction;
            callback?.Invoke();
        }
    }
}