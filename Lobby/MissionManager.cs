using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MissionManager
{
    List<int[]> missionList;
    List<string> missionTexts;
    Dictionary<int, int> missionDic;

    public int ListLength { get {return missionList.Count; }}

    private static MissionManager instance;
    public static MissionManager Instance
    {
        get
        {
            if (instance == null)
                instance = new MissionManager();

            return instance;
        }
    }

    private MissionManager()
    {
        missionList = new List<int[]>();
        missionDic = new Dictionary<int, int>();
        missionTexts = new List<string>();

        TextAsset missions = Resources.Load("missionList") as TextAsset;
        StringReader reader = new StringReader(missions.text);

        while (reader != null)
        {
            string line = reader.ReadLine();
            if (line == null) break;

            string[] datas = line.Split(',');
            int[] missionData = new int[datas.Length];

            switch (datas[0])
            {
                case "Normal":
                    missionData[1] = 0;
                    break;
                case "Challenge":
                    missionData[1] = 1;
                    break;
            }

            for (int i = 1; i < datas.Length - 1; i++)
                missionData[i] = int.Parse(datas[i]);

            missionTexts.Add(datas[datas.Length - 1]);
            missionDic.Add(int.Parse(datas[1]), missionList.Count);
            missionList.Add(missionData);
        }
        reader.Close();
    }

    // 타이틀 화면에서 넘어올 때 초기화
    public void MissionDataLoad()
    {
        for (int i = 0; i < ListLength; i++)
        {
            string temp = "mission" + MissionId(i).ToString();
            string state = temp += "state";
            string count = temp += "count";
            missionList[i][2] = PlayerPrefs.HasKey(state) ? PlayerPrefs.GetInt(state) : 0;
            missionList[i][3] = PlayerPrefs.HasKey(count) ? PlayerPrefs.GetInt(count) : 0;
        }
    }

    // 메인 로비 로드 시 호출
    public void MissionUpdateToList()
    {
        GameObject[] tempMissions = GameObject.Find("LobbyUIManager").GetComponent<LobbyUIManager>().missions;
        for (int i = 0; i < missionList.Count; i++)
        {
            tempMissions[i].GetComponent<MissionData>().GetData(missionList[i]);
        }
    }

    // 메인 로비 언로드 시 호출
    public void MissionUpdateToManager()
    {
        GameObject[] tempMissions = GameObject.Find("LobbyUIManager").GetComponent<LobbyUIManager>().missions;
        for (int i = 0; i < tempMissions.Length; i++)
        {
            MissionData data = tempMissions[i].GetComponent<MissionData>();
            missionList[i][2] = data.curState;
            missionList[i][3] = data.count;
        }
    }

    public void StageClearUpdate(int idx, bool perfect)
    {
        switch(idx)
        {
            case 0:
                PlusMissionCount(1001, 1);
                if(perfect) PlusMissionCount(2001, 1);
                break;
            case 1:
                PlusMissionCount(1002, 1);
                if (perfect) PlusMissionCount(2002, 1);
                break;
            case 2:
                PlusMissionCount(1003, 1);
                if (perfect) PlusMissionCount(2003, 1);
                break;
            case 3:
                PlusMissionCount(1004, 1);
                if (perfect) PlusMissionCount(2004, 1);
                break;
        }
    }

    // 0:category 1:id 2:curState 3:count 4:maxCount 5~:rewardId & rewardNum
    public int CheckMissionState(int id)
    {
        return missionList[missionDic[id]][2];
    }

    public int MissionIdx(int id)
    {
        return missionDic[id];
    }

    public int MissionId(int idx)
    {
        return missionList[idx][1];
    }

    public string GetMissionText(int idx)
    {
        return missionTexts[idx];
    }

    public int[] GetMissionData(int idx)
    {
        return missionList[idx];
    }

    public void PlusMissionCount(int id, int val)
    {
        int idx = missionDic[id];
        if (missionList[idx][2] != 0)
            return;

        int temp = missionList[idx][3] + val;
        if(temp >= missionList[idx][4])
        {
            missionList[idx][3] = missionList[idx][4];
            missionList[idx][2] = 1;
        }
        else
        {
            missionList[idx][3] = temp;
        }

        MissionStateSave(missionList[idx][1], missionList[idx][2], missionList[idx][3]);
    }

    public void MissionStateSave(int id, int state, int count)
    {
        string name = "mission" + id.ToString();
        PlayerPrefs.SetInt(name + "state", state);
        PlayerPrefs.SetInt(name + "count",count);
    }
}