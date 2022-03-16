using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;

    public AudioClip stageBgm;
    public AudioClip stageVictory;
    public AudioClip stageDefeat;
    public AudioClip swordmanAtk;
    public AudioClip archerAtk;
    public AudioClip casterAtk;
    public AudioClip defenderAtk;
    public AudioClip upgrade;
    public AudioClip button;
    public AudioClip die;

    float bgmVol;
    float sfxVol;
    bool bgmOn;
    bool sfxOn;

    private void Awake()
    {
        bgmVol = SoundVolumeManager.Instance.BgmVol;
        sfxVol = SoundVolumeManager.Instance.SfxVol;
        bgmOn = SoundVolumeManager.Instance.BgmOn;
        sfxOn = SoundVolumeManager.Instance.SfxOn;
    }

    void Start()
    {
        bgmPlayer.clip = stageBgm;
        PlayBgm(0);
    }

    // 0:bgm, 1:승리, 2:패배
    public void PlayBgm(int var, float vol = 1)
    {
        if (!bgmOn) return;

        bgmPlayer.volume = bgmVol * vol;
        switch (var)
        {
            case 0:
                bgmPlayer.clip = stageBgm;
                bgmPlayer.loop = true;
                bgmPlayer.Play();
                break;
            case 1:
                bgmPlayer.clip = stageVictory;
                bgmPlayer.loop = false;
                bgmPlayer.Play();
                break;
            case 2:
                bgmPlayer.clip = stageDefeat;
                bgmPlayer.loop = false;
                bgmPlayer.Play();
                break;
        }
    }

    // 0:소드맨, 1:아처, 2:캐스터, 3:디펜더, 4:업그레이드, 5:버튼, 6:사망
    public void PlaySfx(int var, float vol = 1)
    {
        if (!sfxOn) return;

        int idx = 0;
        for(int i = 0; i < sfxPlayer.Length; i++)
            if (!sfxPlayer[i].isPlaying) 
                idx = i;

        sfxPlayer[idx].volume = sfxVol * vol;
        switch (var)
        {
            case 0:
                sfxPlayer[idx].clip = swordmanAtk;
                sfxPlayer[idx].Play();
                break;
            case 1:
                sfxPlayer[idx].clip = archerAtk;
                sfxPlayer[idx].Play();
                break;
            case 2:
                sfxPlayer[idx].clip = casterAtk;
                sfxPlayer[idx].Play();
                break;
            case 3:
                sfxPlayer[idx].clip = defenderAtk;
                sfxPlayer[idx].Play();
                break;
            case 4:
                sfxPlayer[idx].clip = upgrade;
                sfxPlayer[idx].Play();
                break;
            case 5:
                sfxPlayer[idx].clip = button;
                sfxPlayer[idx].Play();
                break;
            case 6:
                sfxPlayer[idx].clip = die;
                sfxPlayer[idx].Play();
                break;
        }
    }

    public void SetVolume(float vol)
    {
        bgmPlayer.volume = vol;
        for (int i = 0; i < sfxPlayer.Length; i++)
            sfxPlayer[i].volume = vol;
    }
}