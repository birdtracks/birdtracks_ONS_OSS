using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public abstract class ObjectList<TObject> : ScriptableObject, IList<TObject>
        where TObject : UnityEngine.Object
    {
        [SerializeField] protected List<TObject> _list = new List<TObject>();

        public TObject this[int index] 
        { 
            get 
            {
                return ((IList<TObject>)_list)[index];
            }
            set 
            { 
                ((IList<TObject>)_list)[index] = value; 
            }
        }

        public int Count 
        {
            get { return ((IList<TObject>)_list).Count; }
        } 

        public bool IsReadOnly
        {
            get { return ((IList<TObject>)_list).IsReadOnly; }
        }

        public void Add(TObject item)
        {
            ((IList<TObject>)_list).Add(item);
        }

        public void Clear()
        {
            ((IList<TObject>)_list).Clear();
        }

        public bool Contains(TObject item)
        {
            return ((IList<TObject>)_list).Contains(item);
        }

        public void CopyTo(TObject[] array, int arrayIndex)
        {
            ((IList<TObject>)_list).CopyTo(array, arrayIndex);
        }

        public IEnumerator<TObject> GetEnumerator()
        {
            return ((IList<TObject>)_list).GetEnumerator();
        }

        public int IndexOf(TObject item)
        {
            return ((IList<TObject>)_list).IndexOf(item);
        }

        public void Insert(int index, TObject item)
        {
            ((IList<TObject>)_list).Insert(index, item);
        }

        public bool Remove(TObject item)
        {
            return ((IList<TObject>)_list).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<TObject>)_list).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<TObject>)_list).GetEnumerator();
        }
    }
}