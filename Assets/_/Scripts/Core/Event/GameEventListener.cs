using UnityEngine;
using UnityEngine.Events;

namespace BirdTracks.Game.Core
{
    public sealed class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEvent m_Event = default;
        [SerializeField] private UnityEvent m_Response = default;



        private void OnEnable()
        {
            m_Event?.Subscribe(OnEventRaisedHandler);
        }

        private void OnDisable()
        {
            m_Event?.Unsubscribe(OnEventRaisedHandler);
        }

        private void OnEventRaisedHandler(GameEvent e)
        {
            m_Response?.Invoke();
        }
    }
}