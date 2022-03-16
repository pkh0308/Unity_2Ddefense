using UnityEngine;
using UnityEngine.UI;

public class LobbySoundManager : MonoBehaviour
{
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;

    public AudioClip stageBgm;
    public AudioClip buttonSfx;
    public AudioClip upgradeSFX;

    public Scrollbar bgmsScrollbar;
    public Scrollbar sfxsScrollbar;
    public Toggle bgmMute;
    public Toggle sfxMute;

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

        bgmsScrollbar.value = bgmVol;
        sfxsScrollbar.value = sfxVol;
        bgmMute.isOn = !bgmOn;
        sfxMute.isOn = !sfxOn;
    }

    void Start()
    {
        bgmPlayer.clip = stageBgm;
        PlayBgm(0);
    }

    private void OnDisable()
    {
        SoundVolumeManager.Instance.SetBgmVol(bgmVol);
        SoundVolumeManager.Instance.SetSfxVol(sfxVol);
        SoundVolumeManager.Instance.SetBgmOn(bgmOn);
        SoundVolumeManager.Instance.SetSfxOn(sfxOn);
    }

    // 0: 로비 bgm
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
        }
    }

    // 0:일반 버튼, 1:업그레이드
    public void PlaySfx(int var, float vol = 1)
    {
        if (!sfxOn) return;

        int idx = 0;
        for (int i = 0; i < sfxPlayer.Length; i++)
            if (!sfxPlayer[i].isPlaying)
                idx = i;

        sfxPlayer[idx].volume = sfxVol * vol;
        switch (var)
        {
            case 0:
                sfxPlayer[idx].clip = buttonSfx;
                sfxPlayer[idx].Play();
                break;
            case 1:
                sfxPlayer[idx].clip = upgradeSFX;
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

    public void SetBgmVolumeInOption(float vol)
    {
        bgmVol = vol;
        bgmPlayer.volume = bgmVol;
    }

    public void SetSfxVolumeInOption(float vol)
    {
        sfxVol = vol;
    }

    public void MuteBgm(bool action)
    {
        if(action == true)
        {
            bgmOn = false;
            bgmPlayer.Stop();
        }
        else
        {
            bgmOn = true;
            bgmPlayer.Play();
        }
    }

    public void MuteSfx(bool action)
    {
        sfxOn = !action;
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("bgmVol", bgmVol);
        PlayerPrefs.SetFloat("sfxVol", sfxVol);
        PlayerPrefs.SetInt("bgmOn", bgmOn == true ? 1 : 0);
        PlayerPrefs.SetInt("sfxOn", sfxOn == true ? 1 : 0);
    }
}