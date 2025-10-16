using UnityEngine;

namespace BirdTracks.Game.Core
{
    [ExecuteInEditMode]
    public abstract class ObjectInstanceInjector<TObject, TContainer> : MonoBehaviour
        where TObject : Component
        where TContainer : ObjectContainer<TObject>
    {
        [SerializeField] private TObject m_Instance = default;
        [SerializeField] private TContainer m_Container = default;
        private bool _added;


        private void Awake()
        {
            if (!Application.isPlaying)
            {
                if (m_Instance == null)
                {
                    m_Instance = GetComponent<TObject>();
                }
            }
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                return;   
            }

            if (_added)
            {
                return;
            }

            _added = true;
            m_Container.Add(m_Instance);
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                return;   
            }

            if (!_added)
            {
                return;
            }

            _added = false;
            m_Container.Remove(m_Instance);
        }
    }
}