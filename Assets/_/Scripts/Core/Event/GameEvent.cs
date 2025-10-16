using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace BirdTracks.Game.Core
{
    [CreateAssetMenu(menuName = "Game/Event")]
    public sealed class GameEvent : ScriptableObject
    {
        private List<UnityAction<GameEvent>> _callbacks = new List<UnityAction<GameEvent>>();


        public void Subscribe(UnityAction<GameEvent> callback)
        {
            _callbacks.Add(callback);
        }

        public void Unsubscribe(UnityAction<GameEvent> callback)
        {
            _callbacks.Remove(callback);
        }

        public void Invoke()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            for (int i = _callbacks.Count; i >= 0; i--)
            {
                if (i >= _callbacks.Count)
                {
                    i = _callbacks.Count;
                    continue;
                }

                UnityAction<GameEvent> callback = _callbacks[i];

                if (callback == null)
                {
                    _callbacks.RemoveAt(i);
                }
                else
                {
                    callback(this);
                }
            }
        }

        public string GetInfoString()
        {
            var builder = new StringBuilder();
            builder.Append( $"Subscribers: {_callbacks.Count}");

            for (int i = 0; i < _callbacks.Count; i++)
            {
                UnityAction<GameEvent> callback = _callbacks[i];
                var unityObject = callback.Target as UnityEngine.Object;
                var objectName = unityObject == null ? "<none>" : unityObject.name;

                builder.AppendLine();
                builder.Append($"Object: {objectName}, Method: {callback.Method.Name}, Type: {callback.Target?.GetType()}");
            }

            return builder.ToString();
        }
    }
}