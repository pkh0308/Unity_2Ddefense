using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public LobbyUIManager uiManager;
    public LobbySoundManager soundManager;

    public GameObject lobbySet;
    public GameObject stagetSelectSet;
    public GameObject stageInfoSet;
    public GameObject stageBackButton;
    public GameObject stageEnterFailSet;
    public GameObject stageLocked;
    public Image stageCostIcon;
    public Text stageCostText;
    public Text stageNameText;
    public Image[] enemyIcons;
    public GameObject[] enemyPlates;
    public Image[] rewardIcons;
    public GameObject[] rewardPlates;
    public StageInfo[] stageInfos;
    StageInfo selectedStageInfo;

    public GameObject loadingScreenSet;
    public GameObject operatorSet;
    public GameObject missionSet;
    public GameObject missionList;
    public GameObject shopSet;
    public GameObject purchaseSet;
    public Text purchaseText;
    public GameObject purchaseResultSet;
    public Text purchaseResultText;
    ShopItemData shopItemData;
    public GameObject exitSet;

    public GameObject cheatSet;
    Dictionary<int, string> shopDic;

    int curStageIdx;

    void Awake()
    {
        loadingScreenSet.SetActive(false);

        shopDic = new Dictionary<int, string>();
        curStageIdx = PlayerPrefs.HasKey("curStageIdx") ? PlayerPrefs.GetInt("curStageIdx") : -1;

        TextAsset items = Resources.Load("shopItemList") as TextAsset;
        StringReader itemsReader = new StringReader(items.text);

        while (itemsReader != null)
        {
            string line = itemsReader.ReadLine();
            if (line == null) break;

            string[] datas = line.Split('$');
            shopDic.Add(int.Parse(datas[0]), datas[1]);
        }
        itemsReader.Close();

        TextAsset stage = Resources.Load("StageInfos") as TextAsset;
        StringReader stageReader = new StringReader(stage.text);

        int idx = 0;
        while (stageReader != null)
        {
            string line = stageReader.ReadLine();
            if (line == null) break;

            string[] datas = line.Split(',');
            stageInfos[idx].stageIdx = int.Parse(datas[0]);
            stageInfos[idx].stageNumber = datas[1];
            stageInfos[idx].stageName = "Stage" + datas[1] + "\n" + datas[2];
            stageInfos[idx].costId = int.Parse(datas[3]);
            stageInfos[idx].costValue = int.Parse(datas[4]);

            int i = 5;
            List<int> enemies = new List<int>();
            while (datas[i].Length > 4)
            {
                enemies.Add(int.Parse(datas[i]));
                i++;
            }
            stageInfos[idx].enemyIds = enemies.ToArray();

            List<int> rewards = new List<int>();
            for (int j = i; j < datas.Length; j++)
            {
                rewards.Add(int.Parse(datas[j]));
            }
            stageInfos[idx].rewardIds = rewards.ToArray();

            idx++;
        }
        stageReader.Close();

        int curAccountExp = PlayerPrefs.HasKey("AccountExp") ? PlayerPrefs.GetInt("AccountExp") : 0;
        float beforeMaxExp = 0;
        TextAsset accountLevel = Resources.Load("AccountLevel") as TextAsset;
        StringReader alReader = new StringReader(accountLevel.text);
        if (alReader.ReadLine() == null) return;

        while (alReader != null)
        {
            string line = alReader.ReadLine();
            if (line == null) break;

            string[] datas = line.Split(',');
            if (int.Parse(datas[2]) <= curAccountExp)
            {
                beforeMaxExp = float.Parse(datas[2]);
                continue;
            }
            else
            {
                float exp = (curAccountExp - beforeMaxExp) / float.Parse(datas[1]);
                uiManager.AccountLevelUpdate(int.Parse(datas[0]), exp);
                break;
            }
        }
        alReader.Close();
    }

    private void Start()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount - 2));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            BackBtn();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MissionManager.Instance.MissionUpdateToList();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StageBtn()
    {
        soundManager.PlaySfx(0);
        stagetSelectSet.SetActive(true);
    }

    public void StageSelectBtn(StageInfo info)
    {
        soundManager.PlaySfx(0);
        // 스테이지 인덱스 및 이름
        selectedStageInfo = info;
        stageCostIcon.sprite = Resources.Load<Sprite>("item" + selectedStageInfo.costId);
        stageCostText.text = string.Format("{0:n0}", selectedStageInfo.costValue);
        stageNameText.text = info.stageName;
        if (selectedStageInfo.stageIdx > curStageIdx + 1)
            stageLocked.SetActive(true);
        else
            stageLocked.SetActive(false);

        // 등장 적
        for (int i = 0; i < enemyIcons.Length; i++)
        {
            if (i < info.enemyIds.Length)
            {
                enemyIcons[i].sprite = Resources.Load<Sprite>("enemy" + info.enemyIds[i]);
                enemyPlates[i].SetActive(true);
            }
            else
                enemyPlates[i].SetActive(false);
        }

        // 보상
        for (int i = 0; i < rewardIcons.Length; i++)
        {
            if (i < info.rewardIds.Length)
            {
                rewardIcons[i].sprite = Resources.Load<Sprite>("item" + info.rewardIds[i]);
                rewardPlates[i].SetActive(true);
            }
            else
                rewardPlates[i].SetActive(false);
        }

        if (!stageInfoSet.activeSelf)
        {
            stageInfoSet.SetActive(true);
            stageBackButton.SetActive(true);
        }
    }

    public void StageSelectBackBtn()
    {
        stageInfoSet.SetActive(false);
        stageBackButton.SetActive(false);
    }

    public void StageStart()
    {
        if (stageLocked.activeSelf) return;

        soundManager.PlaySfx(0);
        if (GoodsManager.Instance.GoodsControl(selectedStageInfo.costId, -(selectedStageInfo.costValue)))
        {
            loadingScreenSet.SetActive(true);
            MissionManager.Instance.MissionUpdateToManager();
            LoadingSceneManager.Instance.StageStart(selectedStageInfo.stageIdx, selectedStageInfo.costValue);
        }
        else
            stageEnterFailSet.SetActive(true);
    }

    public void StageEnterFailBtn()
    {
        stageEnterFailSet.SetActive(false);
    }

    public void OperatorBtn()
    {
        soundManager.PlaySfx(0);
        lobbySet.SetActive(false);
        operatorSet.SetActive(true);
    }

    public void MissionBtn()
    {
        soundManager.PlaySfx(0);
        lobbySet.SetActive(false);
        missionSet.SetActive(true);
        uiManager.MissionAllBtn();
        uiManager.MissionUIUpdate();
    }

    public void ShopBtn()
    {
        soundManager.PlaySfx(0);
        lobbySet.SetActive(false);
        shopSet.SetActive(true);
    }

    public void PurchaseBtn(ShopItemData data)
    {
        soundManager.PlaySfx(0);
        shopItemData = data;
        purchaseText.text = shopDic[data.itemId] + "\n정말 구매하시겠습니까?";
        purchaseSet.SetActive(true);
    }

    public void PurchaseOKBtn()
    {
        soundManager.PlaySfx(0);
        purchaseSet.SetActive(false);
        if (GoodsManager.Instance.GoodsControl(shopItemData.costId, -(shopItemData.costVal)))
        {
            GoodsManager.Instance.GoodsControl(shopItemData.goodsId, shopItemData.goodsVal);
            purchaseResultText.text = "구매가 완료되었습니다.";
            uiManager.GoodsUpdate();
            purchaseResultSet.SetActive(true);
        }
        else
        {
            string temp = shopItemData.costId == 9001 ? "골드가 " : "젬이 ";
            purchaseResultText.text = temp + "부족합니다.";
            purchaseResultSet.SetActive(true);
        }
    }

    public void PurchaseCancelBtn()
    {
        soundManager.PlaySfx(0);
        shopItemData = null;
        purchaseSet.SetActive(false);
    }

    public void PurchaseResultBtn()
    {
        purchaseResultSet.SetActive(false);
    }

    public void BackBtn()
    {
        soundManager.PlaySfx(0);
        lobbySet.SetActive(true);
        if (stagetSelectSet.activeSelf)
        {
            stagetSelectSet.SetActive(false);
            return;
        }
        if (operatorSet.activeSelf)
        {
            operatorSet.SetActive(false);
            return;
        }
        if (missionSet.activeSelf)
        {
            missionSet.SetActive(false);
            missionList.GetComponent<DragController>().PosReset();
            return;
        }
        if (shopSet.activeSelf)
        {
            shopSet.SetActive(false);
            return;
        }
        ExitBtn();
    }

    public void ExitBtn()
    {
        soundManager.PlaySfx(0);
        if (exitSet.activeSelf)
        {
            exitSet.SetActive(false);
        }
        else
        {
            exitSet.SetActive(true);
        }
    }

    public void ExitGame()
    {
        soundManager.PlaySfx(0);
        SaveGame();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void SaveGame()
    {
        GoodsManager.Instance.SaveGoods();

        for (int i = 0; i < uiManager.missions.Length; i++)
        {
            MissionData data = uiManager.GetMissionData(i);
            string temp = "mission" + data.missionId.ToString();
            PlayerPrefs.SetInt(temp + "state", data.curState);
            PlayerPrefs.SetInt(temp + "count", data.count);
        }
        soundManager.SaveVolume();

        PlayerPrefs.Save();
    }

    // 테스트용 치트 버튼
    // 최종 빌드 전에 삭제 요망
    public void CheatSetOpenClose()
    {
        if (cheatSet.activeSelf)
            cheatSet.SetActive(false);
        else
            cheatSet.SetActive(true);
    }

    public void Cheat_GoodsReset()
    {
        GoodsManager.Instance.GoodsControl(9001, -(GoodsManager.Instance.Gold));
        GoodsManager.Instance.GoodsControl(9002, -(GoodsManager.Instance.Gem));
        GoodsManager.Instance.GoodsControl(9003, -(GoodsManager.Instance.Energy));
        uiManager.GoodsUpdate();
    }

    public void Cheat_GoldPlus()
    {
        GoodsManager.Instance.GoodsControl(9001, 10000);
        uiManager.GoodsUpdate();
    }

    public void Cheat_GemPlus()
    {
        GoodsManager.Instance.GoodsControl(9002, 1000);
        uiManager.GoodsUpdate();
    }

    public void Cheat_EnergyPlus()
    {
        GoodsManager.Instance.GoodsControl(9003, 1000);
        uiManager.GoodsUpdate();
    }

    public void Cheat_MissionReset()
    {
        int max = uiManager.missions.Length;
        for (int i = 0; i < max; i++)
        {
            int id = MissionManager.Instance.MissionId(i);
            string state = "mission" + id.ToString() + "state";
            string count = "mission" + id.ToString() + "count";
            PlayerPrefs.SetInt(state, 0);
            PlayerPrefs.SetInt(count, 0);
        }
    }

    public void Cheat_MissionAllClear()
    {
        int max = uiManager.missions.Length;
        for (int i = 0; i < max; i++)
        {
            int id = MissionManager.Instance.MissionId(i);
            int maxCount = MissionManager.Instance.GetMissionData(i)[4];
            string state = "mission" + id.ToString() + "state";
            string count = "mission" + id.ToString() + "count";
            PlayerPrefs.SetInt(state, 1);
            PlayerPrefs.SetInt(count, maxCount);
        }
    }

    public void Cheat_OperUpgradeReset()
    {
        PlayerPrefs.SetInt("swordmanAtk", 0);
        PlayerPrefs.SetInt("swordmanDef", 0);
        PlayerPrefs.SetInt("swordmanCost", 0);

        PlayerPrefs.SetInt("archerDef", 0);
        PlayerPrefs.SetInt("archerCost", 0);
        PlayerPrefs.SetInt("archerAtk", 0);

        PlayerPrefs.SetInt("casterAtk", 0);
        PlayerPrefs.SetInt("casterDef", 0);
        PlayerPrefs.SetInt("casterCost", 0);

        PlayerPrefs.SetInt("defenderAtk", 0);
        PlayerPrefs.SetInt("defenderDef", 0);
        PlayerPrefs.SetInt("defenderCost", 0);
    }

    public void Cheat_AccountLevelReset()
    {
        PlayerPrefs.SetInt("AccountExp", 0);
        uiManager.AccountLevelUpdate(1, 0);
    }
}