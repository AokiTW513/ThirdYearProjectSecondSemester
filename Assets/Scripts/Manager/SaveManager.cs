using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance { get; private set; }

    private string gameSaveFileName = "save.genrakuen";
    private string settingSaveFileName = "setting.genrakuen";
    private string gameSavePath;
    private string settingSavePath;

    public SettingSaveData currentSettingData;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found another SaveManager in this scene.");
            return;
        }

        instance = this;

        string folderPath = Path.Combine(Application.dataPath, "Saves");

        //如果沒資料夾，就幫他建一個
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        gameSavePath = Path.Combine(folderPath, gameSaveFileName);
        settingSavePath = Path.Combine(folderPath, settingSaveFileName);
    }

    public void SaveGame()
    {
        Debug.Log("Save Game.");
    }

    public void SaveSetting()
    {
        //AudioSetting
        currentSettingData.setting.audio.volume.masterVolume = AudioManager.instance.masterVolume;
        currentSettingData.setting.audio.volume.bgmVolume = AudioManager.instance.bgmVolume;
        currentSettingData.setting.audio.volume.sfxVolume = AudioManager.instance.sfxVolume;
        // currentSettingData.setting.audio.volume.ambienceVolume = AudioManager.instance.ambienceVolume;

        // currentSettingData.setting.graph.screenSetting = GameManager.Instance.screenSetting;

        string json = JsonUtility.ToJson(currentSettingData, true);
        File.WriteAllText(settingSavePath, json);

        Debug.Log("Save Setting.");
    }

    public void LoadGame()
    {
        // if (File.Exists(gameSavePath))
        // {
        // }
        // else
        // {
        // }
    }

    public void LoadSetting()
    {
        if (File.Exists(settingSavePath))
        {
            string json = File.ReadAllText(settingSavePath);
            currentSettingData = JsonUtility.FromJson<SettingSaveData>(json);

            Debug.Log("Setting Loaded.");
        }
        else
        {
            Debug.Log("No Game Save Data Found. New Game Data Created.");
            LoadDefaultSetting();
        }
    }

    public bool CheckGameSaveData()
    {
        if (File.Exists(gameSavePath))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public bool CheckSettingSaveData()
    {
        if (File.Exists(settingSavePath))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // public bool IsCollected(string id)
    // {
    //     return currentGameData.map.collectedItemIDs.Contains(id);
    // }

    // public void MarkCollected(string id)
    // {
    //     if (!currentGameData.map.collectedItemIDs.Contains(id))
    //     {
    //         currentGameData.map.collectedItemIDs.Add(id);
    //     }
    // }

    public void LoadDefaultSetting()
    {
        currentSettingData = new SettingSaveData();
        currentSettingData.setting.graph.screenSetting = new ScreenSetting(ScreenSetting.ScreenMode.WindowMode, ScreenSetting.Resolution.Mid);
        currentSettingData.setting.audio.volume.masterVolume = 1;
        currentSettingData.setting.audio.volume.bgmVolume = 1;
        currentSettingData.setting.audio.volume.sfxVolume = 1;

        SaveSetting();

        Debug.Log("Load Default Setting.");
    }

    public void ClearGameSave()
    {
        if (File.Exists(gameSavePath))
        {
            File.Delete(gameSavePath);
        }

        // currentGameData = new GameSaveData();
    }

    public void ClearSettingSave()
    {
        if (File.Exists(settingSavePath))
        {
            File.Delete(settingSavePath);
        }

        currentSettingData = new SettingSaveData();
    }
}