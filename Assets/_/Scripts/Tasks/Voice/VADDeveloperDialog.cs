using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BirdTracks.Game.Core;
using BirdTracks.Game.Tangrams;
using SweetEngine.Extensions;
using SweetEngine.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Birdtracks.Game.ONS
{


    public class VADDeveloperDialog : MonoBehaviour
    {
        [SerializeField] private LoadSceneButton m_LoadSceneButton;
        [SerializeField] private VoiceAssessmentOptionArray m_VADOptions = default;
        [SerializeField] private OptionsShifterElement m_PuzzleOptionsShifter = default;
        public VADOptionShifterOptionElement m_OptionPrefab = default;
        public Transform m_SelectionContainer = default;
        public Transform m_OptionContainer = default;
        public Button m_PlayButton = default;
        
        [SerializeField] private Toggle[] m_SettingsToggles;

        public VADOptionShifterOptionElement _selectedOption = default;
        
        //

        //Control Panel
        [Header("Control Panel")]
        [SerializeField] private string m_ControlPanelGoogleSheet = default;
        [SerializeField] private VADControlPanelOptionElement m_ControlPanelOptionPrefab;
        [SerializeField] private GameObject m_ControlPanel = default;
        [SerializeField] private Transform m_ControlPanelOptionsContainer = default;
        private List<VADControlPanelOptionElement> _controlPanelOptions;
        //

        private void Awake()
        {
            //var difficulties = new HashSet<int>();
            //var sortedOptions = m_Options.ToList();

            for (int i = 0; i < m_VADOptions.Count; i++)
            {
                var option = Instantiate(m_OptionPrefab);

                option.Initialize($"{m_VADOptions[i].VADAssessmentName}", m_VADOptions[i], OnOptionClicked_Button, true, m_VADOptions[i].SceneName);

                option.transform.SetParentAndResetLocal(m_OptionContainer);
            }

            //EDIT: Control Panel
            m_ControlPanel.gameObject.SetActive(false);
            _controlPanelOptions = new List<VADControlPanelOptionElement>();
            ControlPanelUtility.SubscribeData(
                m_ControlPanelGoogleSheet,
                OnDataReady);
            
        }

        private void OnDestroy()
        {
            ControlPanelUtility.Unsubscribe(m_ControlPanelGoogleSheet);
        }

        private void OnDataReady(string[][] csv)
        {
            return;
            /*int storyNameColumn = -1;
            int sceneCollectionColumn = -1;
            int languageColumn = -1;

            for (int x = 0; x < csv[0].Length; x++)
            {
                var column = csv[0][x].Trim().ToLower();
                switch (column)
                {
                    case "story name": storyNameColumn = x; break;
                    case "scene name": sceneCollectionColumn = x; break;
                    case "language": languageColumn = x; break;
                    default:
                        Debug.Log($"Found no column: x({x}) {column}");
                        break;
                }
            }

            CollectionUtility.ResetList(ref _controlPanelOptions, csv.Length - 1);

            if (csv.Length == 0)
            {
                Debug.Log($"No CSV found");
                return;
            }

            for (int y = 1; y < csv.Length; y++)
            {
                var option = Instantiate(m_ControlPanelOptionPrefab);
                option.transform.SetParent(m_ControlPanelOptionsContainer);
                option.transform.localScale = Vector3.one;
                try
                {
                    option.Initialize(
                            OnOptionClickedHandler,
                            csv[y][storyNameColumn],
                            csv[y][sceneCollectionColumn],
                            csv[y][languageColumn]
                        );

                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                    option.SetError();
                }

                _controlPanelOptions.Add(option);
            }*/
        }

        private void OnOptionClickedHandler(VADControlPanelOptionElement option)
        {
            m_ControlPanel.SetActive(false);
            _selectedOption = null;

            for (int i = m_SelectionContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(m_SelectionContainer.GetChild(i).gameObject);
            }

            for (int i = 0; i < option.Settings.StoryData.Count; i++)
            {
                var story = option.Settings.StoryData[i];
                var o = Instantiate(m_OptionPrefab);

                VoiceAssessmentOptions sceneName = m_VADOptions.First(p => p.VADAssessmentName == story.StoryName);
                story.SceneName = sceneName.SceneName;
                Debug.Log(sceneName.SceneName);

                o.Initialize($"{story.StoryName}", story, OnOptionClicked_Button, false, sceneName.SceneName);
                o.transform.SetParentAndResetLocal(m_SelectionContainer);
            }

            CheckPlayButton();
            //CheckStartPosition();
        }

        public void OnShowControlPanel_Clicked()
        {
            m_ControlPanel.SetActive(true);
        }

        public void OnHideControlPanel_Clicked()
        {
            m_ControlPanel.SetActive(false);
        }

        //private void CheckStartPosition()
        //{
        //    StartingPositionDial.Min = 0;
        //    StartingPositionDial.Max = Math.Max(0, OptionContainer.childCount - 1);
        //    StartingPositionDial.CheckValueRange();
        //}

        public void OnShiftOption_Clicked ()
        {
            var puzzle = new VADData()
            {
                StoryName = _selectedOption.m_LabelText.text,
                SceneName = _selectedOption.m_SceneName,
            };

            string optionName = puzzle.StoryName;

            for (int i = 0; i < m_SettingsToggles.Length; i++)
            {
                if(m_SettingsToggles[i].isOn)
                {
                    optionName += ", " + m_SettingsToggles[i].GetComponentInChildren<TextMeshProUGUI>().text;
                }
            }

            Debug.Log(optionName + " " + puzzle.SceneName);

            var option = Instantiate(m_OptionPrefab);
            //option.Initialize($"{puzzle.TangramName}, {puzzle.LitCount}, Break: {puzzle.Break}", puzzle, OnOptionClicked_Button);
            option.Initialize($"{optionName}", puzzle, OnOptionClicked_Button, false, _selectedOption.m_SceneName);
            option.transform.SetParentAndResetLocal(m_SelectionContainer);

            CheckPlayButton();
        }

        public void OnOptionClicked_Button(VADOptionShifterOptionElement option)
        {
            if (_selectedOption != null)
            {
                _selectedOption.Deselect();
                _selectedOption = null;
            }

            _selectedOption = option;
            _selectedOption.Select();
        }

        public void OnShuffleUpClicked_Button()
        {
            if(_selectedOption.transform.parent == m_SelectionContainer)
            {
                _selectedOption.transform.SetSiblingIndex(Mathf.Max(_selectedOption.transform.GetSiblingIndex() - 1, 0));
            }
        }

        public void OnShuffleDownClicked_Button()
        {
            if (_selectedOption.transform.parent == m_SelectionContainer)
            {
                _selectedOption.transform.SetSiblingIndex(Mathf.Min(_selectedOption.transform.GetSiblingIndex() + 1, _selectedOption.transform.parent.childCount - 1));
            }
        }

        public void OnRemoveClicked_Button()
        {
            if (_selectedOption == null || _selectedOption.transform.parent == m_OptionContainer)
            {
                return;
            }

            int currentIndex = _selectedOption.transform.GetSiblingIndex();
            Destroy(_selectedOption.gameObject);

            if (m_SelectionContainer.childCount > 0)
            {
                currentIndex = Mathf.Clamp(currentIndex, 0, m_SelectionContainer.childCount - 1);
                _selectedOption = m_SelectionContainer.GetChild(currentIndex).GetComponent<VADOptionShifterOptionElement>();
                _selectedOption.Select();
            }

            //CheckStartPosition();
            CheckPlayButton();
        }

        private void CheckPlayButton()
        {
            m_PlayButton.interactable = m_SelectionContainer.childCount > 0;
        }

        public void OnPlay_Clicked()
        {
            StartCoroutine(OnPlayClickedRoutine());
        }

        private IEnumerator OnPlayClickedRoutine()
        {
            if (m_SelectionContainer.childCount == 0)
            {
                yield break;
            }
            var puzzles = new VADData[m_SelectionContainer.childCount];
            
            for (int i = 0; i < m_SelectionContainer.childCount; i++)
            {
                var optionElement = m_SelectionContainer.GetChild(i).GetComponent<VADOptionShifterOptionElement>();
    
                if (optionElement.Value is VADData puzzle)
                {
                    puzzles[i] = puzzle;
                }
                else
                {
                    Debug.LogError($"Expected VADData but got {optionElement.Value?.GetType()?.Name ?? "null"}");
                    yield break; // or handle the error appropriately
                }
            }

            var gameSettings = new VADGameSettings
            {
                StoryData = puzzles.ToList()
            };
            
            m_LoadSceneButton.m_SceneName = puzzles[0].SceneName;
            
            yield return m_LoadSceneButton.Load(() =>
            {
                GetComponent<Canvas>().enabled = false;
                FindObjectOfType<VADLevelSequenceHandler>().Initialize(gameSettings, 0);
            });
        }
        
        public MetaDeveloperDialog MetaDialog;
        public void OnQueueMetaClicked()
        {
            var stories = new VADData[m_SelectionContainer.childCount];

            for (int i = 0; i < m_SelectionContainer.childCount; i++)
            {
                var story = (VADData)m_SelectionContainer.GetChild(i).GetComponent<VADOptionShifterOptionElement>().Value;
                stories[i] = story;
            }

            var settings = new VADGameSettings
            {
                StoryData = stories.ToList()
            };
            
            m_LoadSceneButton.m_SceneName = stories[0].SceneName;
            
            MetaDialog.AddPlaySessionItem(new PlaySessionItem
            {
                GameName = "Storytelling",
                Description = "Storytelling",
                LoadCallback = () =>
                {
                    //return m_LoadSceneButton<VA>
                    Debug.Log("Storytelling");
                    return null;
                }
            });
        }
    }
}
