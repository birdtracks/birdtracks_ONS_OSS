using System.Collections;
using UnityEngine;


namespace SweetEngine.Routine
{
    public class WhenCoroutine : CustomYieldInstruction
	{
		private bool _keepWaiting;
		

		public override bool keepWaiting
		{
			get { return _keepWaiting; }
		}


		public WhenCoroutine(Coroutine coroutine)
		{
			_keepWaiting = true;
			CoroutineHost.HostCoroutine(WhenRoutine(coroutine));
		}


        public static implicit operator WhenCoroutine(Coroutine coroutine)
        {
            return new WhenCoroutine(coroutine);
        }


		private IEnumerator WhenRoutine(Coroutine coroutine)
		{
			yield return coroutine;
			_keepWaiting = false;
		}
	}
}
