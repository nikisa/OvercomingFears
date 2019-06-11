using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public static class SoundManager
{
 
    public enum Sound
    {
        PlayerMove,
        PlayerWandAttack,
        PlayerFlashlightAttack,
        PlayerDeath,
        PlayerDeathCrackable,

        StaticIdle,
        StaticDanger,
        StaticAttack,
        StaticDeath,

        ChaserIdle,
        ChaserIdleSleep,
        ChaserActivation,
        ChaserDeactivation,
        ChaserMove,
        ChaserAttack,
        ChaserDeath,

        ArmorDown,
        ArmorUp,

        SwitchOn,
        SwitchOff,

        TriggerOn,
        TriggerOff,
        
        CrackableHalfOpen,
        CrackableOpen,
        CrackableChoked,

        TrapShooting,
        
        ButtonOver,
        ButtonClick
    }

    private static Dictionary<Sound, float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.PlayerMove] = 0;
    }

    
    public static void PlaySound(Sound sound , Vector3 position) //L'audio partirà dalla posizione che diamo
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            audioSource.maxDistance = 100f;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0f;
            audioSource.Play();

            Object.Destroy(soundGameObject, audioSource.clip.length);

        }
    }

    public static void PlaySound(Sound sound)
    {
        if (CanPlaySound(sound))
        {
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("One Shot Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            
            oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
        }
    }

    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            default:
                return true;

            case Sound.PlayerMove:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = .5f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
                break;
                
        }
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.i.soundAudioClipArrays)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + "not found");
        return null;
    }

    public static void AddButtonSounds(this Button_UI buttonUI)
    {
        buttonUI.ClickFunc += () => SoundManager.PlaySound(Sound.ButtonClick);
        buttonUI.MouseOverOnceFunc += () => SoundManager.PlaySound(Sound.ButtonOver);
    }

}
