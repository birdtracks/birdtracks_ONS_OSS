using System.Collections.Generic;
using UnityEngine.Assertions;


namespace SweetEngine.Extensions
{
	public static class ListExtensions
	{
		public static T Random<T>(this IList<T> list)
		{
			Assert.IsNotNull(list);
			Assert.IsTrue(list.Count > 0, "Cannot random from an empty list");

			return list[UnityEngine.Random.Range(0, list.Count)];
		}


		public static T RandomAndRemove<T>(this IList<T> list)
		{
			Assert.IsNotNull(list);
			Assert.IsTrue(list.Count > 0, "Cannot random from an empty list");

			int index = UnityEngine.Random.Range(0, list.Count);
			T ret = list[index];
			list.RemoveAt(index);

			return ret;
		}


		public static T RandomAndRemoveUnsorted<T>(this IList<T> list)
		{
			Assert.IsNotNull(list);
			Assert.IsTrue(list.Count > 0, "Cannot random from an empty list");

			int index = UnityEngine.Random.Range(0, list.Count);
			T ret = list[index];
			int i = list.Count - 1;
			list[index] = list[i];
			list.RemoveAt(i);

			return ret;
		}


		public static void RemoveAtUnsorted<T>(this IList<T> list, int index)
		{
			Assert.IsNotNull(list);
			Assert.IsTrue(list.Count > 0, "Cannot remove from an empty list");
			Assert.IsTrue(index >= 0);
			Assert.IsTrue(index < list.Count);

			if (list.Count > 1)
			{
				int last = list.Count - 1;
				list[index] = list[last];
				list.RemoveAt(last);
			}
		}


		public static bool RemoveUnsorted<T>(this IList<T> list, T obj)
		{
			Assert.IsNotNull(list);

			int index = list.IndexOf(obj);

			if (index == -1)
			{
				return false;
			}

			list.RemoveAtUnsorted(index);
			return true;
		}


		public static void Shuffle<T>(this IList<T> list)
		{
			Assert.IsNotNull(list);

			for (int i = 0; i < list.Count; i++)
			{
				T t = list[i];
				int index = UnityEngine.Random.Range(0, list.Count);
				list[i] = list[index];
				list[index] = t;
			}
		}
	}
}
