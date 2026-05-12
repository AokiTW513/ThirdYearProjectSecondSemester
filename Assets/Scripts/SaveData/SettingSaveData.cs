using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SettingSaveData
{
    public Setting setting = new Setting();
}

[Serializable]
public class Setting
{
    public Audio audio = new Audio();
    public Graph graph = new Graph();
}

[Serializable]
public class Graph
{
    public ScreenSetting screenSetting = new ScreenSetting(ScreenSetting.ScreenMode.WindowMode, ScreenSetting.Resolution.Mid);
}

[Serializable]
public class Audio
{
    public Volume volume = new Volume();
}

[Serializable]
public class Volume
{
    public float masterVolume = 1f;
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;
}