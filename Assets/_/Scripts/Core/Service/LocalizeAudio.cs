using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizeAudio : MonoBehaviour
{
    public string m_Language;

    private Dropdown _dropDown;

    public void Initialize()
    {
        _dropDown = GetComponent<Dropdown>();
        _dropDown.onValueChanged.AddListener(delegate { DropdownValueChanged(_dropDown); });
        _dropDown.value = 0;
        m_Language = _dropDown.options[0].text;
    }

    void DropdownValueChanged (Dropdown change)
    {
        m_Language = change.options[change.value].text;
    }
}
