using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public GameObject loadingScreen;
    
    void Awake()
    {
        SoundVolumeManager.Instance.Initialize();
        LoadingSceneManager.Instance.Initialize();
    }

    public void SetLoadingScreen(bool action)
    {
        loadingScreen.SetActive(action);
    }
}