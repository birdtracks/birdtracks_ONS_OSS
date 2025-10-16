using System;
using System.Collections;
using System.Collections.Generic;


namespace SweetEngine.Collections
{
	public class Deque<T> : ICollection<T>
	{
		private static readonly T[] _EmptyArray = new T[0];
		private T[] _array;
		private int _frontIndex;
		private int _backIndex;
		private int _count;
		private int _version;




		public int Count
		{
			get { return _count; }
		}


		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}


		public T this[int index]
		{
			get { return _array[WrappedIndex(_frontIndex + 1 + index)]; }
		}






		/// <summary>
		/// Initializes a new instance of the <see cref="T:SweetEngine.Collections.Deque`1"/> class that is empty and has the default initial capacity.
		/// </summary>
		public Deque()
			: this(0)
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="T:SweetEngine.Collections.Deque`1"/> class that is empty and has the specified initial capacity.
		/// </summary>
		/// <param name="capacity">The initial number of elements that the <see cref="T:SweetEngine.Collections.Deque`1"/> can contain.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
		public Deque(int capacity)
		{
			if (capacity == 0)
			{
				_array = _EmptyArray;
			}
			else
			{
				_array = new T[capacity];
			}

			_count = 0;
			_frontIndex = capacity - 1;
			_backIndex = 0;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="T:SweetEngine.Collections.Deque`1"/> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
		/// </summary>
		/// <param name="collection">The collection whose elements are copied to the new <see cref="T:SweetEngine.Collections.Deque`1"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is null.</exception>
		public Deque(IEnumerable<T> collection)
			: this(8)
		{
			foreach (var element in collection)
			{
				EnqueueBack(element);
			}
		}




		/// <summary>
		/// Adds an object to the front of the <see cref="T:SweetEngine.Collections.Deque`1"/>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:SweetEngine.Collections.Deque`1"/>. The value can be null for reference types.</param>
		public void EnqueueFront(T item)
		{
			if (_count == _array.Length)
			{
				int capacity = _array.Length*2;

				if (capacity < _array.Length + 4)
				{
					capacity += 4;
				}

				SetCapacity(capacity);
			}

			_array[_frontIndex] = item;
			_frontIndex = WrappedIndex(_frontIndex - 1);
			_count++;
			_version++;
		}


		/// <summary>
		/// Adds an object to the end of the <see cref="T:SweetEngine.Collections.Deque`1"/>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:SweetEngine.Collections.Deque`1"/>. The value can be null for reference types.</param>
		public void EnqueueBack(T item)
		{
			if (_count == _array.Length)
			{
				int capacity = _array.Length * 2;

				if (capacity < _array.Length + 4)
				{
					capacity += 4;
				}

				SetCapacity(capacity);
			}

			_array[_backIndex] = item;
			_backIndex = WrappedIndex(_backIndex + 1);
			_count++;
			_version++;
		}


		/// <summary>
		/// Removes and returns the object at the beginning of the <see cref="T:SweetEngine.Collections.Deque`1"/>.
		/// </summary>
		/// 
		/// <returns>
		/// The object that is removed from the beginning of the <see cref="T:SweetEngine.Collections.Deque`1"/>.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:SweetEngine.Collections.Deque`1"/> is empty.</exception>
		public T DequeueFront()
		{
			if (Count == 0)
			{
				throw new InvalidOperationException("Cannot dequeue from an empty collection");
			}

			_frontIndex = WrappedIndex(_frontIndex + 1);
			T item = _array[_frontIndex];
			_array[_frontIndex] = default(T);
			_count--;
			_version++;

			return item;
		}


		/// <summary>
		/// Removes and returns the object at the end of the <see cref="T:SweetEngine.Collections.Deque`1"/>.
		/// </summary>
		/// 
		/// <returns>
		/// The object that is removed from the end of the <see cref="T:SweetEngine.Collections.Deque`1"/>.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:SweetEngine.Collections.Deque`1"/> is empty.</exception>
		public T DequeueBack()
		{
			if (Count == 0)
			{
				throw new InvalidOperationException("Cannot dequeue from an empty collection");
			}

			_backIndex = WrappedIndex(_backIndex - 1);
			T item = _array[_backIndex];
			_array[_backIndex] = default(T);
			_count--;
			_version++;

			return item;
		}


		/// <summary>
		/// Returns the object at the beginning of the <see cref="T:SweetEngine.Collections.Deque`1"/> without removing it.
		/// </summary>
		/// 
		/// <returns>
		/// The object at the beginning of the <see cref="T:SweetEngine.Collections.Deque`1"/>.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:SweetEngine.Collections.Deque`1"/> is empty.</exception>
		public T PeekFront()
		{
			if (Count == 0)
			{
				throw new InvalidOperationException("Cannot peek into an empty collection");
			}

			return _array[WrappedIndex(_frontIndex + 1)];
		}


		/// <summary>
		/// Returns the object at the end of the <see cref="T:SweetEngine.Collections.Deque`1"/> without removing it.
		/// </summary>
		/// 
		/// <returns>
		/// The object at the end of the <see cref="T:SweetEngine.Collections.Deque`1"/>.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:SweetEngine.Collections.Deque`1"/> is empty.</exception>
		public T PeekBack()
		{
			if (Count == 0)
			{
				throw new InvalidOperationException("Cannot peek into an empty collection");
			}

			return _array[WrappedIndex(_backIndex - 1)];
		}


		public void Clear()
		{
			for (int i = 0; i < _count; ++i)
			{
				_array[WrappedIndex(_frontIndex + 1 + i)] = default(T);
			}

			_count = 0;
			_frontIndex = _array.Length - 1;
			_backIndex = 0;

		}


		public bool Contains(T item)
		{
			for (int i = 0; i < _count; ++i)
			{
				if (_array[WrappedIndex(_frontIndex + i)].Equals(item))
				{
					return true;
				}
			}

			return false;
		}


		public void CopyTo(T[] array, int arrayIndex)
		{

			if (_count > 0)
			{
				int startCopy = WrappedIndex(_frontIndex + 1);
				int endCopy = WrappedIndex(_backIndex - 1);

				if (startCopy < endCopy)
				{
					Array.Copy(_array, startCopy, array, arrayIndex, _count);
				}
				else
				{
					int frontCopyCount = _array.Length - startCopy;

					Array.Copy(_array, startCopy, array, arrayIndex, _array.Length - frontCopyCount);
					Array.Copy(_array, 0, array, arrayIndex + frontCopyCount, _count - frontCopyCount);
				}
			}
		}


		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(this);
		}


		void ICollection<T>.Add(T item)
		{
			throw new InvalidOperationException("Collection is readonly.");
		}


		bool ICollection<T>.Remove(T item)
		{
			throw new InvalidOperationException("Collection is readonly.");
		}


		private void SetCapacity(int capacity)
		{
			var newArray = new T[capacity];

			if (_count > 0)
			{
				int startCopy = WrappedIndex(_frontIndex + 1);
				int endCopy = WrappedIndex(_backIndex - 1);

				if (startCopy < endCopy)
				{
					Array.Copy(_array, startCopy, newArray, 0, _count);
				}
				else
				{
					int frontCopyCount = _array.Length - startCopy;
					Array.Copy(_array, startCopy, newArray, 0, frontCopyCount);
					Array.Copy(_array, 0, newArray, frontCopyCount, _count - frontCopyCount);
				}
			}

			_array = newArray;
			_frontIndex = capacity - 1;
			_backIndex = _count;
		}


		private T GetElement(int index)
		{
			return _array[WrappedIndex(_frontIndex + index)];
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}


		private int WrappedIndex(int index)
		{
			index %= _array.Length;
			
			if (index < 0)
			{
				index += _array.Length;
			}

			return index;
		}



		
		public struct Enumerator : IEnumerator<T>
		{
			private Deque<T> _q;
			private int _index;
			private int _version;
			private T _currentElement;




			public T Current
			{
				get
				{
					if (_index < 0)
					{
						if (_index == -1)
						{
							throw new InvalidOperationException("Enumeration not started.");
						}

						throw new InvalidOperationException("Enumeration has ended.");
					}

					return _currentElement;
				}
			}


			object IEnumerator.Current
			{
				get
				{
					if (_index < 0)
					{
						if (_index == -1)
						{
							throw new InvalidOperationException("Enumeration not started.");
						}

						throw new InvalidOperationException("Enumeration has ended.");
					}
					return _currentElement;
				}
			}




			internal Enumerator(Deque<T> q)
			{
				_q = q;
				_version = _q._version;
				_index = -1;
				_currentElement = default(T);
			}




			public void Dispose()
			{
				_index = -2;
				_currentElement = default(T);
			}

			
			public bool MoveNext()
			{
				if (_version != _q._version)
				{
					throw new InvalidOperationException("Collection modified during enumeration.");
				}

				if (_index == -2)
				{
					return false;
				}

				++_index;

				if (_index == _q._count)
				{
					_index = -2;
					_currentElement = default(T);
					return false;
				}

				_currentElement = _q.GetElement(_index);
				return true;
			}


			void IEnumerator.Reset()
			{
				if (_version != _q._version)
				{
					throw new InvalidOperationException("Collection modified during enumeration.");
				}

				_index = -1;
				_currentElement = default(T);
			}
		}
	}
}
