using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SweetEngine.Collections
{
	public class DictionaryObject<TKey, TValue, TPair> : DictionaryObject, IDictionary<TKey, TValue>, ISerializationCallbackReceiver
		where TPair : DictionaryObjectPair<TKey, TValue>
	{
		[SerializeField] private TPair[] m_Elements = default(TPair[]);
		private Dictionary<TKey, TValue> _dictionary;




		public int Count
		{
			get { return _dictionary.Count; }
		}


		public bool IsReadOnly
		{
			get { return false; }
		}


		public TValue this[TKey key]
		{
			get { return _dictionary[key]; }
			set { _dictionary[key] = value; }
		}


		public ICollection<TKey> Keys
		{
			get { return _dictionary.Keys; }
		}


		public ICollection<TValue> Values
		{
			get { return _dictionary.Values; }
		}





		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) _dictionary).GetEnumerator();
		}


		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).Add(item);
		}


		public void Clear()
		{
			_dictionary.Clear();
		}


		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).Contains(item);
		}


		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
		}


		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).Remove(item);
		}


		public bool ContainsKey(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}


		public void Add(TKey key, TValue value)
		{
			_dictionary.Add(key, value);
		}


		public bool Remove(TKey key)
		{
			return _dictionary.Remove(key);
		}


		public bool TryGetValue(TKey key, out TValue value)
		{
			return _dictionary.TryGetValue(key, out value);
		}


		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}


		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			_dictionary = new Dictionary<TKey, TValue>(m_Elements.Length);

			for (int i = 0; i < m_Elements.Length; i++)
			{
				TPair element = m_Elements[i];
				_dictionary[element.Key] = element.Value;
			}
		}
	}


	public abstract class DictionaryObject : ScriptableObject
	{

	}


	[Serializable]
	public class DictionaryObjectPair<TKey, TValue>
	{
		public TKey Key;
		public TValue Value;
	}
}
