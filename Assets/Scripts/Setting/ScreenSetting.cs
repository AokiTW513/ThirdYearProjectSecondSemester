using System;
using UnityEngine;

[Serializable]
public class ScreenSetting
{
    public ScreenMode screenMode;
    public Resolution resolution;

    public enum ScreenMode
    {
        FullScreen,
        WindowMode,   
    }

    public enum Resolution
    {
        Low,
        Mid,
        High,   
    }

    public ScreenSetting(ScreenMode screenMode, Resolution resolution)
    {
        this.screenMode = screenMode;
        this.resolution = resolution;
    }
}