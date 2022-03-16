using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public AudioSource bgmPlayer;
    public GameObject exitSet;
    public GameObject loadingScreenSet;

    private void Awake()
    {
        bgmPlayer.volume = SoundVolumeManager.Instance.BgmVol;
    }

    private void Start()
    {
        if (SoundVolumeManager.Instance.BgmOn)
            bgmPlayer.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitBtn();
    }

    public void GameStart()
    {
        loadingScreenSet.SetActive(true);
        LoadingSceneManager.Instance.GameStart();
    }


    public void ExitBtn()
    {
        if (exitSet.activeSelf)
            exitSet.SetActive(false);
        else
            exitSet.SetActive(true);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}