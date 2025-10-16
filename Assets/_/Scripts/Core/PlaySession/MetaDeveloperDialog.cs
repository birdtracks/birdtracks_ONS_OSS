using System.Collections.Generic;
using SweetEngine.Extensions;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public sealed class MetaDeveloperDialog : MonoBehaviour
    {
        [SerializeField] private MetaPlaySessionElement m_PlaySessionElement = default;
        [SerializeField] private Transform m_ElementContainer = default;
        [SerializeField] private PlaySessionService PlaySessionService;
        private MetaPlaySessionElement _selectedElement;


        private void Awake()
        {
            for (int i = 0; i < m_ElementContainer.childCount; i++)
            {
                Destroy(m_ElementContainer.GetChild(i).gameObject);
            }
        }
        

        public void AddPlaySessionItem(PlaySessionItem item)
        {
            var element = Instantiate(m_PlaySessionElement);
            element.transform.SetParentAndResetLocal(m_ElementContainer);
            element.Initialize(item, OnElementClickedHandler);
        }

        public bool ContainsQueue(string gameName)
        {
            for (int i = 0; i < m_ElementContainer.childCount; i++)
            {
                if (m_ElementContainer.GetChild(i).GetComponent<MetaPlaySessionElement>().Item.GameName == gameName)
                {
                    return true;
                }
            }

            return false;
        }

        public void RemoveSelectedItemClicked_Button()
        {
            if (_selectedElement == null)
            {
                return;
            }

            for (int i = 0; i < m_ElementContainer.childCount; i++)
            {
                var element = m_ElementContainer.GetChild(i).GetComponent<MetaPlaySessionElement>();
                if (element == _selectedElement)
                {
                    if (i == m_ElementContainer.childCount - 1)
                    {
                        if (m_ElementContainer.childCount > 1)
                        {
                            SetSelectedElement(m_ElementContainer.GetChild(i - 1).GetComponent<MetaPlaySessionElement>());
                        }
                        else
                        {
                            SetSelectedElement(null);
                        }
                    }
                    else
                    {
                        SetSelectedElement(m_ElementContainer.GetChild(i + 1).GetComponent<MetaPlaySessionElement>());                         
                    }
                    
                    Destroy(element.gameObject);
                    return;
                }
            }
        }

        public void ShiftUpElementClicked_Button()
        {
            if (_selectedElement == null)
            {
                return;
            }

            for (int i = 0; i < m_ElementContainer.childCount; i++)
            {
                var child = m_ElementContainer.GetChild(i);
                var element = child.GetComponent<MetaPlaySessionElement>();
                
                if (element == _selectedElement)
                {
                    child.SetSiblingIndex(i - 1);
                }
            }
        }

        public void ShiftDownElementClicked_Button()
        {
            if (_selectedElement == null)
            {
                return;
            }

            for (int i = 0; i < m_ElementContainer.childCount; i++)
            {
                var child = m_ElementContainer.GetChild(i);
                var element = child.GetComponent<MetaPlaySessionElement>();
                
                if (element == _selectedElement)
                {
                    child.SetSiblingIndex(i + 1);
                }
            }
        }

        private void OnElementClickedHandler(MetaPlaySessionElement element)
        {
            SetSelectedElement(element);
        }

        private void SetSelectedElement(MetaPlaySessionElement element)
        {
            if (_selectedElement != null)
            {
                _selectedElement.Deselect();
            }
            
            _selectedElement = element;

            if (_selectedElement != null)
            {
                _selectedElement.Select();
            }
        }

        public void OnPlayClicked_Button()
        {
            if (m_ElementContainer.childCount == 0)
            {
                return;
            }
            
            SetSelectedElement(null);
            var queueItems = new List<PlaySessionItem>();

            for (int i = 0; i < m_ElementContainer.childCount; i++)
            {
                var child = m_ElementContainer.GetChild(i);
                queueItems.Add(child.GetComponent<MetaPlaySessionElement>().Item);
                Destroy(child.gameObject);
            }

            if (queueItems.Count > 0)
            {
                GetComponent<Canvas>().enabled = false;
                PlaySessionService.QueueSessionItems(queueItems);
                PlaySessionService.LoadNextGame();
            }
        }
    }
}