using System.Collections.Generic;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public abstract class ObjectReference
    {

    }

    public abstract class ObjectReference<TObject, TContainer> : ObjectReference
        where TObject : Component
        where TContainer : ObjectContainer<TObject>
    {
        [SerializeField] private TContainer m_Container = default;


        public TObject Instance 
        {
             get { return m_Container.Instance; } 
        }

        public int Count
        {
            get { return m_Container.Count; }
        }


        public void GetInstances(List<TObject> outList)
        {
            m_Container.GetInstances(outList);
        }
    }
}