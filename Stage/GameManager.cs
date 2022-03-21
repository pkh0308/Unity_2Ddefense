using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int stageIdx;
    int stageHp;
    public int maxStageHp;
    public int stageMaxOper;
    int curOper;
    public Transform[] spawners;
    Dictionary<int, string[]> skillDataDictionary;
    List<int[]> stageRewards;

    float curSpawnDelay;
    float nextSpawnDelay;
    public GameObject[] enemies;
    public GameObject dirArrow;

    public GameObject upperGnd;
    public GameObject lowerGnd;
    Operator selectedOper;
    public Image SkillGuage;

    public GameObject pauseSet;
    public GameObject playBtnImage;
    public bool isPaused;
    public Text costText;
    public Image costBar;
    float chargingCost;
    int maxCost;
    int curCost;
    public float costOffset;

    public GameObject exitSet;
    public Text speedText;
    int curSpeed;
    public Text hqHpText;
    public Text enemyCntText;
    public Text maxOperText;
    public Text selectedOperHP;
    public Text selectedOperAtk;
    public Text selectedOperCS;
    public Image selectedOperHPBar;
    public Image selectedOperSkillIcon;
    GameObject attackRange;
    public Image skillActiveTypeIcon;
    public Image skillRecoveryTypeIcon;
    public Text skillNameText;
    public Text skillDescriptionText;
    public Image operPortrait;
    public Sprite[] portraits;

    bool gameOver;
    public GameObject gameOverSet;
    public GameObject resultScreenSet;
    public string stageName;
    public Text stageNameText;
    public GameObject stageClearSet;
    public RewardData[] rewardSet;
    public GameObject operInfoSet;
    public GameObject operInfoSkillType;
    public GameObject operInfoSkillRecoveryType;
    public GameObject operSelectSet;
    public Image[] selectedSkillImage;
    public GameObject blurScreen;

    public ObjectManager objManager;
    public SoundManager soundManager;
    List<Spawn> spawnList;
    public int spawnIndex;
    bool spawnEnd;
    int deadEnemyCnt;

    void Awake()
    {
        spawnList = new List<Spawn>();
        skillDataDictionary = new Dictionary<int, string[]>();
        stageRewards = new List<int[]>();

        Initialize();
    }

    private void Start()
    {
        SceneManager.UnloadSceneAsync((int)LoadingSceneManager.SceneIndex.LOBBY);
    }

    void Update()
    {
        UIUpdate();
        SpawnEnemy();
    }

    void Initialize()
    {
        //status 
        chargingCost = 0;
        curCost = 0;
        maxCost = 99;
        curSpeed = 1;
        stageHp = maxStageHp;

        //enemy spawn
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        stageNameText.text = "Stage " + stageName;

        TextAsset spawnText = Resources.Load("DefenseStageInfo") as TextAsset;
        StringReader reader = new StringReader(spawnText.text);

        while(reader != null)
        {
            string line = reader.ReadLine();
            if (line == null) break;
            if (line != stageIdx.ToString()) continue;

            line = reader.ReadLine();
            while (line.Length > 1)
            {
                string[] datas = line.Split(',');
                Spawn spawnData = new Spawn();
                spawnData.spawnSpot = int.Parse(datas[0]);
                switch (datas[1])
                {
                    case "Melee":
                        spawnData.enemyIndex = 0;
                        break;
                    case "Range":
                        spawnData.enemyIndex = 1;
                        break;
                }
                spawnData.spawnDelay = float.Parse(datas[2]);

                int idx = 0;
                int dirCnt = (datas.Length - 3) / 3;
                spawnData.directions = new Vector3[dirCnt];
                for (int i = 3; i < datas.Length; i = i + 3)
                {
                    float x = float.Parse(datas[i]);
                    float y = float.Parse(datas[i + 1]);
                    float z = float.Parse(datas[i + 2]);

                    spawnData.directions[idx] = new Vector3(x, y, z);
                    idx++;
                }
                spawnList.Add(spawnData);
                line = reader.ReadLine();
                if (line == null) break;
            }
        }
        reader.Close();
        nextSpawnDelay = spawnList[0].spawnDelay;

        //skillData
        TextAsset skillDataText = Resources.Load("skillData") as TextAsset;
        StringReader skillDataReader = new StringReader(skillDataText.text);

        while (skillDataReader != null)
        {
            string line = skillDataReader.ReadLine();
            if (line == null) break;

            string[] datas = line.Split(',');
            skillDataDictionary.Add(int.Parse(datas[0]), new string[] { datas[1], datas[2]});
        }
        skillDataReader.Close();

        //stageReward
        TextAsset stageRewardText = Resources.Load("stageReward") as TextAsset;
        StringReader stageRewardReader = new StringReader(stageRewardText.text);

        while (stageRewardReader != null)
        {
            string line = stageRewardReader.ReadLine();
            if (line == null) break;

            string[] datas = line.Split(',');
            if(int.Parse(datas[0]) != stageIdx) continue;

            for(int i = 1; i < datas.Length; i += 4)
            {
                int[] rwd = new int[4];
                rwd[0] = int.Parse(datas[i]);
                rwd[1] = int.Parse(datas[i + 1]);
                rwd[2] = int.Parse(datas[i + 2]);
                rwd[3] = int.Parse(datas[i + 3]);
                stageRewards.Add(rwd);
            }
            break;
        }
        stageRewardReader.Close();

        for (int i = 0; i < rewardSet.Length; i++)
            rewardSet[i].gameObject.SetActive(false);
    }

    void UIUpdate()
    {
        if (gameOver)
            return;

        //Cost Update
        if(chargingCost < 1)
        {
            chargingCost += Time.deltaTime * costOffset;
        }
        else
        {
            curCost = curCost < maxCost ? curCost + 1 : maxCost;
            chargingCost = 0;
        }

        costBar.rectTransform.localScale = new Vector3(chargingCost, 1, 1);
        costText.text = curCost.ToString();

        //enemyCnt, HQ HP Update
        hqHpText.text = stageHp.ToString();
        enemyCntText.text = spawnIndex + "/" + spawnList.Count;

        //OperLimit Update
        maxOperText.text = "배치 가능한 인원 : " + (stageMaxOper - curOper);

        //Oper Click
        if(selectedOper != null && selectedOper.isDied)
        {
            OperClick(selectedOper);
        }
    }

    void SpawnEnemy()
    {
        if (spawnEnd)
            return;

        if (curSpawnDelay >= nextSpawnDelay)
        {
            curSpawnDelay = 0;
            Spawn spawnData = spawnList[spawnIndex];
            GameObject curArrow = objManager.MakeObj("directionArrow");
            DirectionArrow arrowLogic = curArrow.GetComponent<DirectionArrow>();
            arrowLogic.objManager = objManager;
            arrowLogic.SetEnemyData(this, enemies[spawnData.enemyIndex], spawners[spawnData.spawnSpot].position);
            arrowLogic.SetDirections(spawnData.directions);

            spawnIndex++;
            if (spawnIndex >= spawnList.Count)
            {
                spawnEnd = true;
                return;
            }
            nextSpawnDelay = spawnList[spawnIndex].spawnDelay;
        }
        else
        {
            curSpawnDelay += Time.deltaTime;
        }
    }

    public void EnemyEntrance()
    {
        stageHp--;
        soundManager.PlaySfx(2, 2.0f);
        if (stageHp <= 0)
        {
            TimeControl(1);
            blurScreen.SetActive(true);
            gameOverSet.SetActive(true);
            soundManager.PlayBgm(2);
            objManager.AllStop();
            gameOver = true;
        }
        else
            EnemyCntCheck();
    }

    public void EnemyCntCheck()
    {
        deadEnemyCnt++;
        if (deadEnemyCnt >= spawnList.Count)
        {
            StopAllCoroutines();
            StartCoroutine(StageClear());

            // 미션 체크
            bool perfect = (stageHp == maxStageHp);
            MissionManager.Instance.StageClearUpdate(stageIdx, perfect);
            int curIdx = PlayerPrefs.HasKey("curStageIdx") ? PlayerPrefs.GetInt("curStageIdx") : -1;
            if(curIdx < stageIdx) 
                PlayerPrefs.SetInt("curStageIdx", stageIdx);

            // 계정 경험치 체크
            int curAccountExp = PlayerPrefs.HasKey("AccountExp") ? PlayerPrefs.GetInt("AccountExp") : 0;
            PlayerPrefs.SetInt("AccountExp", curAccountExp + 5);
        }
    }

    public bool CanOperSpawn(int cost)
    {
        if (curCost >= cost && curOper < stageMaxOper)
            return true;
        else
            return false;
    }

    public void OperCount(int n)
    {
        curOper += n;
    }

    public void CostSpend(int cost)
    {
        if(curCost >= cost)
        {
            curCost -= cost;
        }
    }

    public void TileControl(Vector3 pos, Operator.Type type)
    {
        if (type == Operator.Type.Melee)
        {
            TilemapController lowerTile = lowerGnd.GetComponent<TilemapController>();
            lowerTile.RespawnTile(pos);
        }
        else if (type == Operator.Type.Range)
        {
            TilemapController upperTile = upperGnd.GetComponent<TilemapController>();
            upperTile.RespawnTile(pos);
        }
    }

    // 0 입력 시 현재속도로 복원
    public void TimeControl(float scale)
    {
        if (scale == 0)
            Time.timeScale = curSpeed;
        else
            Time.timeScale = scale;
    }

    IEnumerator StageClear()
    {
        yield return new WaitForSeconds(0.5f);
        TimeControl(1);
        stageClearSet.SetActive(true);
        soundManager.PlayBgm(1);
        objManager.AllStop();
        DiceReward();
        gameOver = true;

        yield return new WaitForSeconds(4.0f);
        stageClearSet.SetActive(false);
        blurScreen.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        resultScreenSet.SetActive(true);
    }

    void DiceReward()
    {
        int idx = 0;
        for(int i = 0; i < stageRewards.Count; i++)
        {
            if (Random.Range(1, 10001) > stageRewards[i][3]) 
                continue;

            int id = stageRewards[i][0];
            int val = Random.Range(stageRewards[i][1], stageRewards[i][2] + 1);

            // 재화 지급
            if(GoodsManager.Instance.GoodsControl(id, val) == false)
                Debug.Log("Goods Control Failed...");

            // UI에 적용
            rewardSet[idx].rewardIcon.sprite = Resources.Load<Sprite>("item" + id.ToString());
            rewardSet[idx].rewardCount.text = string.Format("{0:n0}", val);
            rewardSet[idx].gameObject.SetActive(true);

            idx++;
        }
    }

    //////////////     UI functions       //////////////   

    public void PauseBtn()
    {
        soundManager.PlaySfx(5, 0.7f);
        if (pauseSet.activeSelf)
        {
            Time.timeScale = curSpeed;
            playBtnImage.SetActive(false);
            pauseSet.SetActive(false);
            isPaused = false;
            soundManager.SetVolume(1.0f);
        }
        else
        {
            Time.timeScale = 0;
            playBtnImage.SetActive(true);
            pauseSet.SetActive(true);
            isPaused = true;
            soundManager.SetVolume(0.5f);
        }
    }

    public void SpeedBtn()
    {
        soundManager.PlaySfx(5, 0.7f);
        if (curSpeed == 1)
        {
            speedText.text = "x2";
            curSpeed = 2;
        }
        else
        {
            speedText.text = "x1";
            curSpeed = 1;
        }

        if (!isPaused)
            Time.timeScale = curSpeed;
    }

    public void ExitBtn()
    {
        if(!exitSet.activeSelf)
        {
            exitSet.SetActive(true);
            isPaused = true;
            Time.timeScale = 0;
            soundManager.SetVolume(0.5f);
        }
        else
        {
            exitSet.SetActive(false);
            if(!pauseSet.activeSelf)
            {
                isPaused = false;
                Time.timeScale = curSpeed;
                soundManager.SetVolume(0.5f);
            }
        }
    }

    public void ExitYes()
    {
        Time.timeScale = 1;
        LoadingSceneManager.Instance.StageExit();
    }

    public void GameOverBtn()
    {
        gameOverSet.SetActive(false);
        resultScreenSet.SetActive(true);
    }

    public void ResultBtn()
    {
        resultScreenSet.SetActive(false);
        blurScreen.SetActive(false);
        LoadingSceneManager.Instance.StageExit();
    }

    public void OperClick(Operator oper)
    {
        if(operInfoSet.activeSelf)
        {
            OperInfoClose();
        }
        else
        {
            selectedOper = oper;
            operPortrait.sprite = portraits[selectedOper.operID / 10000 - 1];
            foreach (Image img in selectedSkillImage)
            {
                string skillNum = "skill" + selectedOper.skillNum.ToString();
                img.sprite = Resources.Load<Sprite>(skillNum) as Sprite;
            }

            Operator operLogic = selectedOper.GetComponent<Operator>();
            if(operLogic.type == Operator.Type.Range)
            {
                attackRange = objManager.MakeObj("attackRange");
                float operRange = selectedOper.GetComponent<Operator>().attackRange * 1.8f;
                attackRange.transform.localScale = new Vector3(operRange, operRange, 1);
                attackRange.transform.position = selectedOper.transform.position;
            }

            switch(operLogic.skillType)
            {
                case Operator.SkillType.Active:
                    skillActiveTypeIcon.sprite = Resources.Load<Sprite>("ActiveUse") as Sprite;
                    break;
                case Operator.SkillType.Auto:
                    skillActiveTypeIcon.sprite = Resources.Load<Sprite>("AutoUse") as Sprite;
                    break;
            }
            switch (operLogic.skillRecoveryType)
            {
                case Operator.SkillRecoveryType.Auto:
                    skillRecoveryTypeIcon.sprite = Resources.Load<Sprite>("AutoRecovery") as Sprite;
                    break;
                case Operator.SkillRecoveryType.Attack:
                    skillRecoveryTypeIcon.sprite = Resources.Load<Sprite>("AttackRecovery") as Sprite;
                    break;
            }
            skillNameText.text = skillDataDictionary[operLogic.skillNum][0];
            skillDescriptionText.text = skillDataDictionary[operLogic.skillNum][1];

            if (selectedOper != null)
            {
                selectedOperHPBar.rectTransform.localScale = new Vector3(selectedOper.health / selectedOper.maxHealth, 1, 1);
                selectedOperHP.text = selectedOper.health.ToString() + " / " + selectedOper.maxHealth.ToString();
                selectedOperAtk.text = "공격력 " + selectedOper.AttackPower.ToString() + "    방어력 " + selectedOper.DefensePower.ToString();
                selectedOperCS.text = selectedOper.type == Operator.Type.Melee ? "저지 " + selectedOper.canStop.ToString() : "저지 0";

                if (selectedOper.sp < selectedOper.maxSp)
                {
                    SkillGuage.rectTransform.localScale = new Vector3(1, selectedOper.sp / selectedOper.maxSp, 1);
                    selectedOperSkillIcon.color = new Color(0.3f, 0.3f, 0.3f, 1);
                }
                else
                {
                    SkillGuage.rectTransform.localScale = new Vector3(1, 0, 1);
                    selectedOperSkillIcon.color = new Color(1, 1, 1, 1);
                }
            }

            TimeControl(0.3f);
            operInfoSet.SetActive(true);
            operSelectSet.transform.position = Camera.main.WorldToScreenPoint(oper.gameObject.transform.position);
            operSelectSet.SetActive(true);
            soundManager.SetVolume(0.5f);
        }
    }

    public void OperInfoClose()
    {
        TimeControl(0);
        selectedOper = null;
        operInfoSet.SetActive(false);
        operSelectSet.SetActive(false);
        if(attackRange!= null)
            attackRange.SetActive(false);
        soundManager.SetVolume(1.0f);
    }

    public void OperCancel()
    {
        OperCount(-1);
        curCost += selectedOper.SpawnCost;
        TileControl(selectedOper.transform.position, selectedOper.type);
        selectedOper.OperUiOff(false);
        GameObject oper = selectedOper.gameObject;
        oper.SetActive(false);

        selectedOper.TargetClear();
        OperClick(selectedOper);
    }

    public void SkillBtn()
    {
        if (selectedOper.sp < selectedOper.maxSp || selectedOper.skillType == Operator.SkillType.Auto)
            return;
        else
        {
            selectedOper.skiilActive();
            OperClick(selectedOper);
        }
    }

    public bool CheckGameOvered()
    {
        return gameOver;
    }
}