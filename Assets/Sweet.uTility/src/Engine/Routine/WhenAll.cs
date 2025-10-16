using UnityEngine;


namespace SweetEngine.Routine
{
    public class WhenAll<T> : CustomYieldInstruction
		where T : CustomYieldInstruction
	{
		private T[] _yields;
		
		 
		public override bool keepWaiting
		{
			get
			{
				for (int i = 0; i < _yields.Length; i++)
				{
					var customYieldInstruction = _yields[i];

					if (customYieldInstruction.keepWaiting)
					{
						return true;
					}
				}

				return false;
			}
		}


		public WhenAll(params T[] yields)
		{
			_yields = yields;
		}
	}
}
