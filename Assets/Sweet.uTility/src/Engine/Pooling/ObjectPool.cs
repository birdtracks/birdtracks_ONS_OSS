using System;
using System.Collections.Generic;
using UnityEngine.Assertions;


namespace SweetEngine.Pooling
{
    public class ObjectPool<T> : IPool<T>
    {
        private readonly object _sync = new object();
        private readonly Stack<T> _stack;
        private readonly Func<T> _constructor;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;




        public int CountAll
        {
            get;
            private set;
        }


        public int CountActive
        {
            get
            {
                lock (_sync)
                {
                    return CountAll - _stack.Count;
                }
            }
        }


        public int CountPooled
        {
            get
            {
                lock (_sync)
                {
                    return _stack.Count;
                }
            }
        }




        public ObjectPool(Func<T> constructor, Action<T> onGet, Action<T> onRelease)
        {
            Assert.IsNotNull(constructor);

            _stack = new Stack<T>();
            _constructor = constructor;
            _onGet = onGet;
            _onRelease = onRelease;
        }




        public T Get()
        {
            T t;

            lock (_sync)
            {
                if (_stack.Count == 0)
                {
                    t = _constructor();
                    CountAll++;
                }
                else
                {
                    t = _stack.Pop();
                }
            }

            if (_onGet != null)
            {
                _onGet(t);
            }

            return t;
        }


        public void Release(T obj)
        {
            if (_onRelease != null)
            {
                _onRelease(obj);
            }

            lock (_sync)
            {
                _stack.Push(obj);
            }
        }


        public void Clear()
        {
            Assert.IsTrue(CountActive == 0, "Cannot clear while pool objects are active.");

            lock (_sync)
            {
                _stack.Clear();
            }
        }
    }
}
