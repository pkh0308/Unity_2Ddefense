using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUIManager : MonoBehaviour
{
    public LobbySoundManager soundManager;
    public LobbyManager lobbyManager;
    public DragController dragController;

    public TopBar[] topBars;

    public GameObject[] missions;
    public Image[] buttons;
    public TextMeshProUGUI[] missionTexts;
    public Image[] missionRewardIcons;
    public Text[] missionRewardTexts;

    public UpgradeData[] upgradeDatas;
    public Text[] upgradeTexts;
    public Text[] upgradeCostTexts;
    public Image[] upgradeCostIcons;
    public GameObject upgradeFailedSet;
    public GameObject optionSet;

    public GameObject missionRedDot;
    public Text missionRedDotText;

    public Text accountLevel;
    public Image accountExpBar;
    int maxEnergy;
    int secondsPerEnergyCharging;
    public GameObject autoChargePopup;
    public Text remainTimeText;

    int[] swordmanStatus;
    int[] archerStatus;
    int[] casterStatus;
    int[] defenderStatus;
    int[] swordmanCost;
    int[] archerCost;
    int[] casterCost;
    int[] defenderCost;

    public GameObject[] clearedImgs;
    Vector2[] missionsPos;
    int curMissionCategory;

    public ShopItemData[] shopItems;

    // 저장된 오퍼레이터 스테이터스를 불러와 배열에 저장
    // 오퍼레이터 업그레이드 화면에서 사용
    void Awake()
    {
        swordmanStatus = new int[3];
        archerStatus = new int[3];
        casterStatus = new int[3];
        defenderStatus = new int[3];
        swordmanCost = new int[3];
        archerCost = new int[3];
        casterCost = new int[3];
        defenderCost = new int[3];

        swordmanStatus[0] = PlayerPrefs.HasKey("swordmanAtk") ? PlayerPrefs.GetInt("swordmanAtk") : 0;
        swordmanStatus[1] = PlayerPrefs.HasKey("swordmanDef") ? PlayerPrefs.GetInt("swordmanDef") : 0;
        swordmanStatus[2] = PlayerPrefs.HasKey("swordmanCost") ? PlayerPrefs.GetInt("swordmanCost") : 0;

        archerStatus[0] = PlayerPrefs.HasKey("archerAtk") ? PlayerPrefs.GetInt("archerAtk") : 0;
        archerStatus[1] = PlayerPrefs.HasKey("archerDef") ? PlayerPrefs.GetInt("archerDef") : 0;
        archerStatus[2] = PlayerPrefs.HasKey("archerCost") ? PlayerPrefs.GetInt("archerCost") : 0;

        casterStatus[0] = PlayerPrefs.HasKey("casterAtk") ? PlayerPrefs.GetInt("casterAtk") : 0;
        casterStatus[1] = PlayerPrefs.HasKey("casterDef") ? PlayerPrefs.GetInt("casterDef") : 0;
        casterStatus[2] = PlayerPrefs.HasKey("casterCost") ? PlayerPrefs.GetInt("casterCost") : 0;

        defenderStatus[0] = PlayerPrefs.HasKey("defenderAtk") ? PlayerPrefs.GetInt("defenderAtk") : 0;
        defenderStatus[1] = PlayerPrefs.HasKey("defenderDef") ? PlayerPrefs.GetInt("defenderDef") : 0;
        defenderStatus[2] = PlayerPrefs.HasKey("defenderCost") ? PlayerPrefs.GetInt("defenderCost") : 0;
    }

    void Start()
    {
        curMissionCategory = 0;
        missionsPos = new Vector2[missions.Length];
        for (int i = 0; i < missions.Length; i++)
        {
            missionsPos[i] = missions[i].GetComponent<RectTransform>().anchoredPosition;
        }

        EnergyCharge();
        GoodsUpdate();
        ShopUpdate();
        MissionDataUpdate();
        OperatorUpgradeUpdate();
        RedDotUpdate();
    }
    
    // 상점, 재화 관련

    // 상품 정보 갱신
    void ShopUpdate()
    {
        for(int i = 0; i < shopItems.Length; i++)
        {
            string item = "";
            switch(shopItems[i].goodsId)
            {
                case 9001:
                    item = "골드 ";
                    break;
                case 9002:
                    item = "젬 ";
                    break;
                case 9003:
                    item = "에너지 ";
                    break;
            }
            item += shopItems[i].goodsVal.ToString();
            shopItems[i].itemText.text = string.Format("{0:n0}", item);

            shopItems[i].costImg.sprite = Resources.Load<Sprite>("item" + shopItems[i].costId.ToString());
            shopItems[i].costText.text = string.Format("{0:n0}", shopItems[i].costVal.ToString());
        }
    }

    // 마지막 저장된 시간과 현재 시간을 비교하여 에너지 자동 충전(TimeSpan.TotalSeconds 이용)
    // 현재 에너지가 최대에너지 이상일 경우 스킵
    public void EnergyCharge()
    {
        if (GoodsManager.Instance.Energy >= maxEnergy) return;

        DateTime now = DateTime.Now;
        TimeSpan span = now - new DateTime(2022, 4, 3, 0, 0, 0);
        int timeGap = ((int)span.TotalSeconds - PlayerPrefs.GetInt("LastAccess")) / secondsPerEnergyCharging;
        int remainTime = ((int)span.TotalSeconds - PlayerPrefs.GetInt("LastAccess")) % secondsPerEnergyCharging;
        if (GoodsManager.Instance.ChargeEnergy(timeGap) == false)
            Debug.Log("Error Occured while goods control...");
        StartCoroutine(AutoChageEnergy(remainTime));

        now = DateTime.Now;
        span = now - new DateTime(2022, 4, 3, 0, 0, 0);
        PlayerPrefs.SetInt("LastAccess", (int)span.TotalSeconds);
    }

    public void GoodsUpdate()
    {
        for (int i = 0; i < topBars.Length; i++)
        {
            topBars[i].goldText.text = string.Format("{0:n0}", GoodsManager.Instance.Gold.ToString());
            topBars[i].gemText.text = string.Format("{0:n0}", GoodsManager.Instance.Gem.ToString());
            topBars[i].energyText.text = string.Format("{0:n0}", GoodsManager.Instance.Energy.ToString() + "/" + maxEnergy);
        }
    }

    // 에너지 자동 충전
    // 현재 에너지가 최대치보다 작은 동안 루프 반복
    // 텍스트는 팝업이 활성화 상태일때만 갱신
    IEnumerator AutoChageEnergy(int time)
    {
        while(GoodsManager.Instance.Energy < maxEnergy)
        {
            if (autoChargePopup.activeSelf)
            {
                int mins = time / 60;
                int seconds = time % 60;
                remainTimeText.text = string.Format("{0,1} : {1,2}", mins.ToString("D1"), seconds.ToString("D2"));
            }

            if (time > 0) time--;
            else
            {
                time = 179;
                GoodsManager.Instance.ChargeEnergy(1);
                GoodsUpdate();
            }

            yield return new WaitForSeconds(1.0f);
        }
        remainTimeText.text = "-:--";
    }

    // 계정 레벨 및 최대에너지 갱신
    public void AccountLevelUpdate(int level, float exp, int seconds)
    {
        accountLevel.text = "Lv." + string.Format("{0:n0}", level);
        accountExpBar.rectTransform.localScale = new Vector3(exp, 1, 1);
        maxEnergy = GoodsManager.Instance.MaxEnergy;
        secondsPerEnergyCharging = seconds;
    }

    // 레드닷 갱신
    public void RedDotUpdate()
    {
        // 미션
        int missionCnt = 0;
        for (int i = 0; i < missionsPos.Length; i++)
        {
            if (missions[i].GetComponent<MissionData>().curState == 1)
                missionCnt++;
        }
        if (missionCnt > 0)
        {
            missionRedDotText.text = string.Format("{0:n0}", missionCnt);
            missionRedDot.SetActive(true);
        }
        else
            missionRedDot.SetActive(false);
    }

    // 미션 관련


    public MissionData GetMissionData(int idx)
    {
        return missions[idx].GetComponent<MissionData>();
    }

    public void MissionUIUpdate()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            int state = missions[i].GetComponent<MissionData>().curState;
            if (state == 0)
                buttons[i].color = Color.gray;
            else if (state == 2)
                clearedImgs[i].SetActive(true);
        }
        MissionSort();
    }

    void MissionDataUpdate()
    {
        for(int i = 0; i < missions.Length; i++)
        {
            int[] data = MissionManager.Instance.GetMissionData(i);
            missionTexts[i].text = string.Format("{0:n0}", MissionManager.Instance.GetMissionText(i));
            missionRewardIcons[i].sprite = Resources.Load<Sprite>("item" + data[5].ToString());
            missionRewardTexts[i].text = string.Format("{0:n0}", data[6].ToString());
        }
    }

    // 미션 목록을 다음과 같은 순서로 정렬
    // 1순위 : 완료 후 보상 수령하지 않은 미션 -> 2순위 : 아직 클리어하지 않은 미션 -> 3순위 : 보상 수령까지 마친 미션
    void MissionSort()
    {
        Vector2 beforePos = Vector2.one;

        // 1순위 : 완료 후 보상 수령하지 않은 미션 
        for (int i = 0; i < missionsPos.Length; i++)
        {
            if (!missions[i].activeSelf) continue;

            if (missions[i].GetComponent<MissionData>().curState != 1)
                continue;
            else
            {
                RectTransform rect = missions[i].GetComponent<RectTransform>();
                if (beforePos == Vector2.one)
                    rect.anchoredPosition = missionsPos[0];
                else
                    rect.anchoredPosition = beforePos + Vector2.down * 350;

                beforePos = rect.anchoredPosition;
            }
        }

        // 2순위 : 아직 클리어하지 않은 미션
        for (int i = 0; i < missionsPos.Length; i++)
        {
            if (!missions[i].activeSelf) continue;

            if (missions[i].GetComponent<MissionData>().curState != 0)
                continue;
            else
            {
                RectTransform rect = missions[i].GetComponent<RectTransform>();
                if (beforePos == Vector2.one)
                    rect.anchoredPosition = missionsPos[0];
                else
                    rect.anchoredPosition = beforePos + Vector2.down * 350;

                beforePos = rect.anchoredPosition;
            }
        }
        // 3순위 : 보상 수령까지 마친 미션
        for (int i = 0; i < missionsPos.Length; i++)
        {
            if (!missions[i].activeSelf) continue;

            if (missions[i].GetComponent<MissionData>().curState != 2)
                continue;
            else
            {
                RectTransform rect = missions[i].GetComponent<RectTransform>();
                if (beforePos == Vector2.one)
                    rect.anchoredPosition = missionsPos[0];
                else
                    rect.anchoredPosition = beforePos + Vector2.down * 350;

                beforePos = rect.anchoredPosition;
            }
        }
    }

    // 모든 미션 목록 노출
    // 이미 해당 카테고리일 경우 미동작
    public void MissionAllBtn()
    {
        if (curMissionCategory == 0) return;

        soundManager.PlaySfx(0);
        curMissionCategory = 0;
        for (int i = 0; i < missions.Length; i++)
        {
            missions[i].SetActive(true);
            RectTransform rect = missions[i].GetComponent<RectTransform>();
            rect.anchoredPosition = missionsPos[i];
        }
        dragController.limitPos = new Vector3(700, (missions.Length - 4) * 350, 0);
        dragController.PosReset();
        MissionSort();
    }

    // 미션 카테고리가 Normal인 미션들만 활성화
    public void MissionNormalBtn()
    {
        if (curMissionCategory == 1) return;

        soundManager.PlaySfx(0);
        curMissionCategory = 1;
        Vector2 beforePos = Vector2.one;
        int count = 0;

        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].GetComponent<MissionData>().category == MissionData.Category.Normal)
            {
                missions[i].SetActive(true);

                RectTransform rect = missions[i].GetComponent<RectTransform>();
                if (beforePos == Vector2.one)
                    rect.anchoredPosition = missionsPos[0];
                else
                    rect.anchoredPosition = beforePos + Vector2.down * 350;

                beforePos = rect.anchoredPosition;
                count++;
            }
            else
                missions[i].SetActive(false);
        }
        int yPos = count >= 4 ? (count - 4) * 350 : -250;
        dragController.limitPos = new Vector3(700, yPos, 0);
        dragController.PosReset();
        MissionSort();
    }

    // 미션 카테고리가 Challenge인 미션들만 활성화
    public void MissionChallengeBtn()
    {
        if (curMissionCategory == 2) return;

        soundManager.PlaySfx(0);
        curMissionCategory = 2;
        Vector2 beforePos = Vector2.one;
        int count = 0;

        for (int i = 0; i < missions.Length; i++)
        {
            if (missions[i].GetComponent<MissionData>().category == MissionData.Category.Challenge)
            {
                missions[i].SetActive(true);

                RectTransform rect = missions[i].GetComponent<RectTransform>();
                if (beforePos == Vector2.one)
                    rect.anchoredPosition = missionsPos[0];
                else
                    rect.anchoredPosition = beforePos + Vector2.down * 350;

                beforePos = rect.anchoredPosition;
                count++;
            }
            else
                missions[i].SetActive(false);
        }
        int yPos = count >= 4 ? (count - 4) * 350 : -250;
        dragController.limitPos = new Vector3(700, yPos, 0);
        dragController.PosReset();
        MissionSort();
    }

    // 미션 보상 수령 버튼용 함수
    // 현재 미션 상태가 1(완료, 보상 미수령)이 아니면 미동작 
    public void MissionCompleteBtn(MissionData data)
    {
        if (data.curState != 1) return;

        soundManager.PlaySfx(0);
        if (!GoodsManager.Instance.GoodsControl(data.RewardId, data.RewardNum))
            Debug.Log("error occured");

        data.curState = 2;
        int idx = MissionManager.Instance.MissionIdx(data.missionId);
        clearedImgs[idx].SetActive(true);
        GoodsUpdate();
        MissionSort();
        MissionManager.Instance.MissionStateSave(data.missionId, data.curState, data.count);
    }

    // 오퍼레이터 업그레이드 화면 갱신(전체)
    // operatorStatus, operatorUpgradeCost 를 읽어온 뒤 Awake()에서 배열에 저장해둔 레벨값을 인덱스로 이용
    void OperatorUpgradeUpdate()
    {
        TextAsset status = Resources.Load("operatorStatus") as TextAsset;
        StringReader statusReader = new StringReader(status.text);
        TextAsset cost = Resources.Load("operatorUpgradeCost") as TextAsset;
        StringReader costReader = new StringReader(cost.text);

        int idx = 0;
        int[] targetStatus = swordmanStatus;
        int[] targetCost = swordmanCost;
        while (statusReader != null)
        {
            string line_s = statusReader.ReadLine();
            string line_c = costReader.ReadLine();
            if (line_s == null) break;

            int s_idx = idx % 3;
            switch (line_s)
            {
                case "10001":
                    targetStatus = swordmanStatus;
                    targetCost = swordmanCost;
                    break;
                case "20001":
                    targetStatus = archerStatus;
                    targetCost = archerCost;
                    break;
                case "30001":
                    targetStatus = casterStatus;
                    targetCost = casterCost;
                    break;
                case "40001":
                    targetStatus = defenderStatus;
                    targetCost = defenderCost;
                    break;
                default:
                    string[] statusData = line_s.Split(',');
                    string[] costData = line_c.Split(',');

                    upgradeDatas[idx].curLevel = targetStatus[s_idx];
                    upgradeDatas[idx].maxLevel = statusData.Length - 1;
                    upgradeDatas[idx].costId = int.Parse(costData[0]);
                    upgradeDatas[idx].costVal = upgradeDatas[idx].curLevel == upgradeDatas[idx].maxLevel ? 0 : int.Parse(costData[targetStatus[s_idx] + 1]);

                    switch (s_idx)
                    {
                        case 0:
                            string tempAtk = targetStatus[0] < upgradeDatas[idx].maxLevel ? targetStatus[0].ToString() : "Max";
                            upgradeTexts[idx].text = "공격력 증가(Lv." + tempAtk + ")";
                            break;
                        case 1:
                            string tempDef = targetStatus[1] < upgradeDatas[idx].maxLevel ? targetStatus[1].ToString() : "Max";
                            upgradeTexts[idx].text = "방어력 증가(Lv." + tempDef + ")";
                            break;
                        case 2:
                            string tempCost = targetStatus[2] < upgradeDatas[idx].maxLevel ? targetStatus[2].ToString() : "Max";
                            upgradeTexts[idx].text = "코스트 감소(Lv." + tempCost + ")";
                            break;
                    }

                    if(upgradeDatas[idx].curLevel < upgradeDatas[idx].maxLevel)
                    {
                        upgradeCostTexts[idx].text = string.Format("{0:n0}", costData[targetStatus[s_idx] + 1]);
                        upgradeCostIcons[idx].sprite = Resources.Load<Sprite>("item" + costData[0]);
                    }
                    else
                    {
                        upgradeCostTexts[idx].gameObject.SetActive(false);
                        upgradeCostIcons[idx].gameObject.SetActive(false);
                    }

                    idx++;
                    break;
            }
        }
        statusReader.Close();
        costReader.Close();
    }

    // 오퍼레이터 업그레이드 화면 갱신(일부)
    // 업그레이드 버튼 사용 후 갱신을 위해 호출
    void OperatorUpgradeUpdate(int operId, int idx)
    {
        int[] targetStatus = swordmanStatus;
        switch (operId)
        {
            case 10001:
                targetStatus = swordmanStatus;
                break;
            case 20001:
                targetStatus = archerStatus;
                idx += 3;
                break;
            case 30001:
                targetStatus = casterStatus;
                idx += 6;
                break;
            case 40001:
                targetStatus = defenderStatus;
                idx += 9;
                break;
        }

        switch (idx % 3)
        {
            case 0:
                string tempAtk = targetStatus[0] < upgradeDatas[idx].maxLevel ? targetStatus[0].ToString() : "Max";
                upgradeTexts[idx].text = "공격력 증가(Lv." + tempAtk + ")";
                break;
            case 1:
                string tempDef = targetStatus[1] < upgradeDatas[idx].maxLevel ? targetStatus[1].ToString() : "Max";
                upgradeTexts[idx].text = "방어력 증가(Lv." + tempDef + ")";
                break;
            case 2:
                string tempCost = targetStatus[2] < upgradeDatas[idx].maxLevel ? targetStatus[2].ToString() : "Max";
                upgradeTexts[idx].text = "코스트 감소(Lv." + tempCost + ")";
                break;
        }
        upgradeCostTexts[idx].text = string.Format("{0:n0}", upgradeDatas[idx].costVal);

        if(upgradeDatas[idx].curLevel == upgradeDatas[idx].maxLevel)
        {
            upgradeCostTexts[idx].gameObject.SetActive(false); 
            upgradeCostIcons[idx].gameObject.SetActive(false);
        }
    }

    // 업그레이드 버튼용 함수
    // 재화가 충분할 경우 OperatorUpgradeUpdate(변수 2개) 호출하여 데이터 갱신
    // 부족할 경우 실패 팝업 노출
    public void OperUpgradeBtn(UpgradeData data)
    {
        // 이미 Max일 경우 스킵
        if (data.curLevel == data.maxLevel)
            return;

        // 가격 체크 후 데이터 업데이트
        if (GoodsManager.Instance.GoodsControl(data.costId, -(data.costVal)))
        {
            soundManager.PlaySfx(1);
            GoodsUpdate();

            int[] target = swordmanStatus;
            int idx = 0;
            string name = "";
            switch(data.operId)
            {
                case 10001:
                    target = swordmanStatus;
                    name = "swordman";
                    break;
                case 20001:
                    target = archerStatus;
                    name = "archer";
                    break;
                case 30001:
                    target = casterStatus;
                    name = "caster";
                    break;
                case 40001:
                    target = defenderStatus;
                    name = "defender";
                    break;
            }
            switch(data.category)
            {
                case UpgradeData.UpagradeCategory.Atk:
                    idx = 0;
                    name += "Atk";
                    break;
                case UpgradeData.UpagradeCategory.Def:
                    idx = 1;
                    name += "Def";
                    break;
                case UpgradeData.UpagradeCategory.Cost:
                    idx = 2;
                    name += "Cost";
                    break;
            }
            target[idx]++;
            data.curLevel = target[idx];
            if (data.curLevel == data.maxLevel)
                data.costVal = 0;
            else
            {
                TextAsset costAsset = Resources.Load("operatorUpgradeCost") as TextAsset;
                StringReader costReader = new StringReader(costAsset.text);

                while (costReader != null)
                {
                    string line = costReader.ReadLine();
                    if (line != data.operId.ToString()) continue;

                    for(int i = 0; i <= idx; i++)
                        line = costReader.ReadLine();
                    string[] values = line.Split(',');
                    data.costVal = int.Parse(values[target[idx] + 1]);
                    break;
                }
                costReader.Close();
            }

            // 현재 스탯 레벨 저장 및 갱신
            PlayerPrefs.SetInt(name, data.curLevel);
            OperatorUpgradeUpdate(data.operId, idx);
        }
        else
        {
            soundManager.PlaySfx(0);
            upgradeFailedSet.SetActive(true);
        }
    }

    public void UpgradeFailBtn()
    {
        upgradeFailedSet.SetActive(false);
    }

    public void AutoChagePopupBtn()
    {
        if (autoChargePopup.activeSelf)
            autoChargePopup.SetActive(false);
        else
            autoChargePopup.SetActive(true);
    }

    // 환경설정
    public void OptionOpenBtn()
    {
        soundManager.PlaySfx(0);
        optionSet.SetActive(true);
    }

    public void OptionExitBtn()
    {
        soundManager.PlaySfx(0);
        optionSet.SetActive(false);
    }
}