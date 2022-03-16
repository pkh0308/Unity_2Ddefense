using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVolumeManager 
{
    // ½Ì±ÛÅæ ±¸Çö
    private static SoundVolumeManager instance;
    public static SoundVolumeManager Instance
    {
        get
        {
            if (instance == null)
                instance = new SoundVolumeManager();

            return instance;
        }
    }

    // º¯¼ö ¼±¾ð ¹× »ý¼ºÀÚ
    float bgmVol;
    public float BgmVol { get { return bgmVol; } }
    float sfxVol;
    public float SfxVol { get { return sfxVol; } }
    bool bgmOn;
    public bool BgmOn { get { return bgmOn; } }
    bool sfxOn;
    public bool SfxOn { get { return sfxOn; } }

    private SoundVolumeManager()
    {
        bgmVol = PlayerPrefs.HasKey("bgmVol") ? PlayerPrefs.GetFloat("bgmVol") : 1.0f;
        sfxVol = PlayerPrefs.HasKey("sfxVol") ? PlayerPrefs.GetFloat("sfxVol") : 1.0f;
        bgmOn = PlayerPrefs.HasKey("bgmOn") ? PlayerPrefs.GetInt("bgmOn") == 1 : true;
        sfxOn = PlayerPrefs.HasKey("sfxOn") ? PlayerPrefs.GetInt("sfxOn") == 1 : true;
    }

    public void Initialize()
    {

    }

    // º¼·ý ¹× ¹ÂÆ® ¼¼ÆÃ
    public void SetBgmVol(float vol)
    {
        bgmVol = vol;
    }

    public void SetSfxVol(float vol)
    {
        sfxVol = vol;
    }

    public void SetBgmOn(bool action)
    {
        bgmOn = action;
    }

    public void SetSfxOn(bool action)
    {
        sfxOn = action;
    }
}