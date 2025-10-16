using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SweetEngine.Utility
{
	public static class CollectionUtility
	{
		public static void ResetList<T>(ref List<T> list, int minCapacity)
			where T : Component
		{
			ResetList(ref list, minCapacity, Object.Destroy);
		}


		public static void ResetList(ref List<GameObject> list, int minCapacity)
		{
			ResetList(ref list, minCapacity, Object.Destroy);
		}


		public static void ResetList<T>(ref List<T> list, int minCapacity, Action<GameObject> cleanupCallback)
			where T : Component
		{
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					var c = list[i];

					if (c != null)
					{
						cleanupCallback(c.gameObject);
					}
				}

				list.Clear();

				if (list.Capacity < minCapacity)
				{
					list.Capacity = minCapacity;
				}
			}
			else
			{
				list = new List<T>(minCapacity);
			}
		}


		public static void ResetList(ref List<GameObject> list, int minCapacity, Action<GameObject> cleanupCallback)
		{
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					var g = list[i];

					if (g != null)
					{
						cleanupCallback(g);
					}
				}

				list.Clear();

				if (list.Capacity < minCapacity)
				{
					list.Capacity = minCapacity;
				}
			}
			else
			{
				list = new List<GameObject>(minCapacity);
			}
		}


		public static void ResetArray<T>(ref T[] array, int length)
			where T : Component
		{
			ResetArray(ref array, length, Object.Destroy);
		}


		public static void ResetArray(ref GameObject[] array, int length)
		{
			ResetArray(ref array, length, Object.Destroy);
		}


		public static void ResetArray<T>(ref T[] array, int length, Action<GameObject> cleanupCallback)
			where T : Component
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					var c = array[i];

					if (c != null)
					{
						cleanupCallback(c.gameObject);
					}
				}

				if (array.Length == length)
				{
					return;
				}
			}

			array = new T[length];
		}


		public static void ResetArray(ref GameObject[] array, int length, Action<GameObject> cleanupCallback)
		{
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					var g = array[i];

					if (g != null)
					{
						cleanupCallback(g);
					}
				}

				if (array.Length == length)
				{
					return;
				}
			}

			array = new GameObject[length];
		}
	}
}
