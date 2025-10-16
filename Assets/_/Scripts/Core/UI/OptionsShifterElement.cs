using SweetEngine.Extensions;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using TMPro;

namespace BirdTracks.Game.Core
{
    public sealed class OptionsShifterElement : MonoBehaviour
    {
        [SerializeField] private OptionsShifterOptionElement m_OptionPrefab = default;
        [SerializeField] private RectTransform m_OptionsContainer = default;
        [SerializeField] private RectTransform m_SelectionContainer = default;
        [SerializeField] private Button m_ShiftButton = default;
        [SerializeField] private Button m_OrderUpButton = default;
        [SerializeField] private Button m_OrderDownButton = default;
        private List<OptionValue> _values = new List<OptionValue>();
        public List<GameObject> _optionItems = new List<GameObject>();
        public RectTransform OptionsContainer { get { return m_OptionsContainer; } }
        //public OptionValue[] _values;
        public OptionsShifterOptionElement _selectedOption;
        public bool UseSortIndex;
        private bool _isSingle;

        private void OnEnable()
        {
            m_ShiftButton.onClick.AddListener(OnOptionShiftClicked_Button);
            m_OrderUpButton.onClick.AddListener(OnOrderUpClicked_Button);
            m_OrderDownButton.onClick.AddListener(OnOrderDownClicked_Button);
        }

        private void OnDisable()
        {
            m_ShiftButton.onClick.RemoveListener(OnOptionShiftClicked_Button);
            m_OrderUpButton.onClick.RemoveListener(OnOrderUpClicked_Button);
            m_OrderDownButton.onClick.RemoveListener(OnOrderDownClicked_Button);
        }

        public void OnOptionShiftClicked_Button()
        {
            if (_selectedOption == null)
            {
                return;
            }

            if (_selectedOption.transform.parent == m_OptionsContainer)
            {
                MoveOptionFromContainerToContainer(m_OptionsContainer, m_SelectionContainer);
            }
            else
            {
                MoveOptionFromContainerToContainer(m_SelectionContainer, m_OptionsContainer);
            }
        }

        public void OnOrderUpClicked_Button()
        {
            _selectedOption.transform.SetSiblingIndex(Mathf.Max(_selectedOption.transform.GetSiblingIndex() - 1, 0));
        }

        public void OnOrderDownClicked_Button()
        {
            _selectedOption.transform.SetSiblingIndex(Mathf.Min(_selectedOption.transform.GetSiblingIndex() + 1, _selectedOption.transform.parent.childCount - 1));
        }

        public void Initialize(OptionValue[] values, bool useSortIndex = false)
        {
            UseSortIndex = true;
            _values.AddRange(values);
            ResetOptions();
        }

        public void InitializeSingle(OptionValue[] values, bool useSortIndex = false)
        {
            UseSortIndex = true;
            _values.AddRange(values);
            _isSingle = true;
            ResetOptionsSingle();
        }

        public void ResetOptions()
        {
            ClearContainer(m_OptionsContainer);
            ClearContainer(m_SelectionContainer);

            if (_values == null)
            {
                return;
            }

            for (int i = 0; i < _values.Count; i++)
            {
                var optionValue = _values[i];
                var optionElement = Instantiate(m_OptionPrefab);
                optionElement.name = _values[i].Label;
                optionElement.OnOptionSelected += OnOptionSelectedHandler;
                optionElement.Initialize(optionValue.Label, optionValue.Value, optionValue.SortIndex);
                optionElement.transform.SetParentAndResetLocal(m_OptionsContainer);
                _optionItems.Add(optionElement.gameObject);
            }

            _selectedOption = null;
            RefreshButtons();
        }

        public void ResetOptionsSingle()
        {
            //ClearContainer(m_OptionsContainer);
            ClearContainer(m_SelectionContainer);

            if (_values == null)
            {
                return;
            }

            var optionValue = _values[_values.Count - 1];
            var optionElement = Instantiate(m_OptionPrefab);
            optionElement.name = _values[_values.Count - 1].Label;
            optionElement.OnOptionSelected += OnOptionSelectedHandler;
            optionElement.Initialize(optionValue.Label, optionValue.Value);
            optionElement.transform.SetParentAndResetLocal(m_OptionsContainer);

            _selectedOption = null;
            RefreshButtons();
        }

        private void OnOptionSelectedHandler(OptionsShifterOptionElement option)
        {
            _selectedOption?.Deselect();
            _selectedOption = option;
            _selectedOption.Select();

            RefreshButtons();
        }

        private void RefreshButtons()
        {
            if (_selectedOption == null)
            {
                m_ShiftButton.interactable = false;
                m_OrderDownButton.interactable = false;
                m_OrderUpButton.interactable = false;
            }
            else
            {
                if (_selectedOption.transform.parent == m_OptionsContainer)
                {
                    m_ShiftButton.interactable = true;
                    m_OrderDownButton.interactable = false;
                    m_OrderUpButton.interactable = false;
                }
                else
                {
                    m_ShiftButton.interactable = true;
                    m_OrderDownButton.interactable = true;
                    m_OrderUpButton.interactable = true;
                }
            }
        }

        private static void ClearContainer(RectTransform container)
        {
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                Destroy(container.GetChild(i));
            }
        }

        public List<T> GetValues<T>()
        {
            var ret = new List<T>();

            for (int i = 0; i < m_SelectionContainer.childCount; i++)
            {
                ret.Add((T)m_SelectionContainer.GetChild(i).GetComponent<OptionsShifterOptionElement>().Value);
            }

            return ret;
        }

        public void ClearSelectedOptions()
        {
            for (int i = m_SelectionContainer.childCount - 1; i >= 0; i--)
            {
                OnOptionSelectedHandler(m_SelectionContainer.GetChild(i).GetComponent<OptionsShifterOptionElement>());
                OnOptionShiftClicked_Button();
            }
        }

        public void SelectValueWhere<T>(Func<T, bool> predicate)
        {
            for (int i = 0; i < m_OptionsContainer.childCount; i++)
            {
                var element = m_OptionsContainer.GetChild(i).GetComponent<OptionsShifterOptionElement>();

                if (predicate((T)element.Value))
                {
                    OnOptionSelectedHandler(element);
                    OnOptionShiftClicked_Button();
                }
            }
        }

        public void SelectNameWhere(string name)
        {
            for (int i = 0; i < m_OptionsContainer.childCount; i++)
            {
                Debug.Log(name);

                var element = m_OptionsContainer.GetChild(i).GetComponent<OptionsShifterOptionElement>();
                var elementName = m_OptionsContainer.GetChild(i).name;

                if (elementName.Contains(name))
                {
                    OnOptionSelectedHandler(element);
                    OnOptionShiftClicked_Button();
                }
            }
        }

        public void SetValueWhere<T>(Func<T, bool> predicate)
        {
            ClearSelectedOptions();

            var optionsToShit = new List<OptionsShifterOptionElement>();

            for (int i = 0; i < m_OptionsContainer.childCount; i++)
            {
                var element = m_OptionsContainer.GetChild(i).GetComponent<OptionsShifterOptionElement>();

                if (predicate((T)element.Value))
                {
                    optionsToShit.Add(element);
                }
            }

            foreach (var element in optionsToShit)
            {
                OnOptionSelectedHandler(element);
                OnOptionShiftClicked_Button();
            }
        }


        private void MoveOptionFromContainerToContainer(RectTransform source, RectTransform target)
        {
            int currentIndex = _selectedOption.transform.GetSiblingIndex();
            _selectedOption.transform.SetParent(target);
            SortOptions();
            _selectedOption?.Deselect();
            _selectedOption = null;

            if (source.transform.childCount > 0)
            {
                currentIndex = Mathf.Clamp(currentIndex, 0, source.transform.childCount - 1);
                _selectedOption = source.transform.GetChild(currentIndex).GetComponent<OptionsShifterOptionElement>();
                _selectedOption.Select();
            }
        }

        private List<OptionsShifterOptionElement> _OptionsScratch = new List<OptionsShifterOptionElement>(64);
        private void SortOptions()
        {
            if (_selectedOption.transform.parent == m_OptionsContainer)
            {
                for (int Index = 0; Index < m_OptionsContainer.childCount; ++Index)
                {
                    _OptionsScratch.Add(m_OptionsContainer.GetChild(Index).GetComponent<OptionsShifterOptionElement>());
                }

                if (UseSortIndex)
                {
                    _OptionsScratch.Sort(SortBySortIndex);
                }
                else
                {
                    _OptionsScratch.Sort(SortTransformByName);
                }

                for (int Index = _OptionsScratch.Count - 1; Index >= 0; --Index)
                {
                    _OptionsScratch[Index].transform.SetSiblingIndex(Index);
                }

                _OptionsScratch.Clear();

                // _optionItems.Sort(SortByName);
                // _selectedOption.transform.SetSiblingIndex(_optionItems.IndexOf(_selectedOption.gameObject));
            }

        }

        private static int SortByName(GameObject obj1, GameObject obj2)
        {
            return obj1.name.CompareTo(obj2.name);
        }

        private static int SortTransformByName(OptionsShifterOptionElement obj1, OptionsShifterOptionElement obj2)
        {
            return obj1.name.CompareTo(obj2.name);
        }

        private static int SortBySortIndex(OptionsShifterOptionElement obj1, OptionsShifterOptionElement obj2)
        {
            return obj1.SortIndex.CompareTo(obj2.SortIndex);
        }

        public void ClearOptionsContainer()
        {
            _selectedOption = null;
            _optionItems.Clear();

            for (int i = 0; i < m_OptionsContainer.childCount; i++)
            {
                Destroy(m_OptionsContainer.GetChild(0).gameObject);
            }
        }

        public struct OptionValue
        {
            public string Label;
            public System.Object Value;
            public bool UseSortIndex;
            public int SortIndex;
        }
    }
}