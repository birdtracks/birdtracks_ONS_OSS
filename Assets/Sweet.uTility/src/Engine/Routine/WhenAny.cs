using UnityEngine;


namespace SweetEngine.Routine
{
	public class WhenAny : CustomYieldInstruction
	{
		private CustomYieldInstruction[] _yields;




		public override bool keepWaiting
		{
			get
			{
				for (int i = 0; i < _yields.Length; i++)
				{
					var customYieldInstruction = _yields[i];

					if (!customYieldInstruction.keepWaiting)
					{
						return true;
					}
				}

				return false;
			}
		}




		public WhenAny(CustomYieldInstruction[] yields)
		{
			_yields = yields;
		}
	}
}
