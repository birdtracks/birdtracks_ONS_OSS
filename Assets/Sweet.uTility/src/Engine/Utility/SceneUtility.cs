using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SweetEngine.Utility
{
	public static class SceneUtility
	{
		public static T[] FindAllComponentsInScene<T>(Scene scene)
			where T : Component
		{
			var ret = new List<T>();
			var allLoadedBehaviours = Resources.FindObjectsOfTypeAll<T>();

			for (int i = 0; i < allLoadedBehaviours.Length; i++)
			{
				var behaviour = allLoadedBehaviours[i];

				if (behaviour.gameObject.scene != scene)
				{
					continue;
				}

				ret.Add(behaviour);
			}

			return ret.ToArray();
		}
	}
}
