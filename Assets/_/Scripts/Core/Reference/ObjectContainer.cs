using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public abstract class ObjectContainer<TObject> : ScriptableObject, IList<TObject>
        where TObject : Component
    {
        private List<TObject> _instances = new List<TObject>();


        public TObject Instance 
        {
             get { return _instances.Count > 0 ? _instances[0] : null; } 
        }

        bool ICollection<TObject>.Remove(TObject item)
        {
            return _instances.Remove(item);
        }

        public int Count
        {
            get { return _instances.Count; }
        }

        public bool IsReadOnly => false;

        public int IndexOf(TObject item)
        {
            return _instances.IndexOf(item);
        }

        public void Insert(int index, TObject item)
        {
            _instances.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _instances.RemoveAt(index);
        }

        public TObject this[int index]
        {
            get => _instances[index];
            set => _instances[index] = value;
        }


        public void GetInstances(List<TObject> outList)
        {
            outList.AddRange(_instances);
        }
        public List<TObject> GetInstances()
        {
            return new List<TObject>(_instances);
        }

        public void Add(TObject instance)
        {
            _instances.Add(instance);
        }

        public void Clear()
        {
            _instances.Clear();
        }

        public bool Contains(TObject item)
        {
            return _instances.Contains(item);
        }

        public void CopyTo(TObject[] array, int arrayIndex)
        {
            _instances.CopyTo(array, arrayIndex);
        }

        public void Remove(TObject instance)
        {
            _instances.Remove(instance);
        }

        public IEnumerator<TObject> GetEnumerator()
        {
            return _instances.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _instances).GetEnumerator();
        }
    }
}