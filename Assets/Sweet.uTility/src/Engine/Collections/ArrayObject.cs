using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SweetEngine.Collections
{
	public class ArrayObject<TElement> : ArrayObject, IList<TElement>
	{
		[SerializeField] private TElement[] m_Array = default(TElement[]);




		public int Length
		{
			get
			{
				return m_Array.Length;
			}
		}


		public TElement this[int index]
		{
			get { return m_Array[index]; }
			set { m_Array[index] = value; }
		}


		int ICollection<TElement>.Count
		{
			get { return m_Array.Length; }
		}


		public bool IsReadOnly
		{
			get { return false; }
		}




		IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
		{
			return ((IList<TElement>)m_Array).GetEnumerator();
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_Array.GetEnumerator();
		}


		void IList<TElement>.RemoveAt(int index)
		{
			((IList<TElement>)m_Array).RemoveAt(index);
		}


		void ICollection<TElement>.Add(TElement item)
		{
			((IList<TElement>)m_Array).Add(item);
		}


		void ICollection<TElement>.Clear()
		{
			((IList<TElement>)m_Array).Clear();
		}


		bool ICollection<TElement>.Contains(TElement item)
		{
			return ((IList<TElement>)m_Array).Contains(item);
		}


		public void CopyTo(TElement[] array, int arrayIndex)
		{
			m_Array.CopyTo(array, arrayIndex);
		}


		bool ICollection<TElement>.Remove(TElement item)
		{
			return ((IList<TElement>)m_Array).Remove(item);
		}


		int IList<TElement>.IndexOf(TElement item)
		{
			return ((IList<TElement>)m_Array).IndexOf(item);
		}


		void IList<TElement>.Insert(int index, TElement item)
		{
			((IList<TElement>)m_Array).Insert(index, item);
		}
	}


	public abstract class ArrayObject : ScriptableObject
	{

	}
}
