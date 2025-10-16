using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

namespace Birdtracks.Game.ONS.GreenDress
{
    public class StoryQuestionDataHolder : MonoBehaviour, IPointerClickHandler, INotificationReceiver
    {
        [SerializeField] private QuestionData _questionData;

        private bool _interactable = false;
        

        public event Action<GameObject> OnClick; 

        public void SetQuestionData(QuestionData qd)
        {
            _questionData = qd;
        }

        public bool IsCorrectAnswer()
        {
            return _questionData.IsCorrectAnswer;
        }

        public void SetHolderInteractable()
        {
            _interactable = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_interactable) return;
            
            Debug.Log("clicked on " + eventData.pointerPress.name);
            OnClick?.Invoke(eventData.pointerPress);
            _interactable = false;
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            //SetHolderInteractable();
        }
    }
}
