using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VADGameSettings
{
    public List<VADData> StoryData;
}

public struct VADData
{
    public string StoryName;
    public string SceneName;
    public string Language;
}