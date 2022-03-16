using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager
{
    public static LoadingSceneManager instance;
    public static LoadingSceneManager Instance
    {
        get
        {
            if (instance == null)
                instance = new LoadingSceneManager();

            return instance;
        }
    }
    int stageCost;
    
    public enum SceneIndex
    {
        TITLE = 0,
        LOBBY = 1,
        STAGE1_1 = 2,
        STAGE1_2 = 3,
        STAGE1_3 = 4,
        STAGE1_4 = 5
    }

    public void Initialize()
    {
        SceneManager.LoadScene((int)SceneIndex.TITLE, LoadSceneMode.Additive);
    }

    public void GameStart()
    {
        MissionManager.Instance.MissionDataLoad();
        SceneManager.LoadScene((int)SceneIndex.LOBBY, LoadSceneMode.Additive);
    }

    public void StageStart(int idx, int cost)
    {
        stageCost = cost;
        SceneManager.LoadScene(idx + 2, LoadSceneMode.Additive);
    }

    public void StageExit()
    {
        SceneManager.LoadScene((int)SceneIndex.LOBBY, LoadSceneMode.Additive);
    }

    public void UnloadScene(int idx)
    {
        SceneManager.UnloadSceneAsync(idx + 2);
    }

    public int GetStageCost()
    {
        return stageCost;
    }
}