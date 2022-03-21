using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionData : MonoBehaviour
{
    public int missionId;
    public enum Category { Normal, Challenge }
    public Category category;

    // 0 : not cleraed. 1 : cleared(not complete), 2 : completed
    public int curState;
    public int count;
    public int maxCount;

    // 9001 : gold, 9002 : gem, 9003 : energy
    int rewardId;
    int rewardNum;
    public int RewardId {  get { return rewardId; } }
    public int RewardNum { get { return rewardNum; } }

    public void GetData(int[] data)
    {
        curState = data[2];
        count = data[3];
        maxCount = data[4];
        rewardId = data[5];
        rewardNum = data[6];
    }
}