using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float bgmVolume = 1;
    // [Range(0, 1)]
    // public float ambienceVolume = 1;
    [Range(0, 1)]
    public float sfxVolume = 1;

    private Bus masterBus;
    private Bus bgmBus;
    // private Bus ambienceBus;
    private Bus sfxBus;

    public static AudioManager instance { get; private set; }
    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> studioEventEmitters;
    private EventInstance ambienceEventInstance;
    private EventInstance bgmEventInstance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found another AudioManager in this scene.");
            return;
        }

        instance = this;

        eventInstances = new List<EventInstance>();
        studioEventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        bgmBus = RuntimeManager.GetBus("bus:/BGM");
        // ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    public void LoadVolumeData()
    {
        //調這裡的音量
        masterVolume = SaveManager.instance.currentSettingData.setting.audio.volume.masterVolume;
        bgmVolume = SaveManager.instance.currentSettingData.setting.audio.volume.bgmVolume;
        sfxVolume = SaveManager.instance.currentSettingData.setting.audio.volume.sfxVolume;
        // ambienceVolume = SaveManager.instance.currentSettingData.setting.audio.volume.ambienceVolume;

        //設定FMOD的音量
        masterBus.setVolume(masterVolume);
        bgmBus.setVolume(bgmVolume);
        // ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(sfxVolume);
        // if(Level1UIController.instance != null)
        // {
        //     Level1UIController.instance.LoadBGMSFXASliderValue(bgmVolume, sfxVolume);
        // }

        Debug.Log($"已將音量設定為音樂:{bgmVolume},音效:{sfxVolume}");
    }

    public void SetVolume()
    {
        masterBus.setVolume(masterVolume);
        bgmBus.setVolume(bgmVolume);
        // ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(sfxVolume);
        Debug.Log($"已將音量設定為音樂:{bgmVolume},音效:{sfxVolume}");
        // GameManager.instance.SaveSettingData();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        SetVolume();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        SetVolume();
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
        Debug.Log("Play One Shot: " + sound);
    }

    public void InitializedAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }
    
    public void InitializedBGM(EventReference BGMEventReference)
    {
        if(bgmEventInstance.isValid())
        {
            bgmEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            bgmEventInstance.release();

            eventInstances.Remove(bgmEventInstance);
        }

        bgmEventInstance = CreateInstance(BGMEventReference);

        bgmEventInstance.start();

        Debug.Log("Now Playing: " + BGMEventReference);
    }

    public float GetEventLength(EventReference eventRef)
    {
        var desc = RuntimeManager.GetEventDescription(eventRef);
        desc.getLength(out int lengthMS);
        return lengthMS / 1000f;
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        studioEventEmitters.Add(emitter);
        return emitter;
    }

    public void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        foreach (StudioEventEmitter studioEventEmitter in studioEventEmitters)
        {
            studioEventEmitter.Stop();
        }

        Debug.Log("Cleaned up all audio instances and emitters.");
    }
}