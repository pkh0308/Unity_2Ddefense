using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsManager
{
    private static GoodsManager instance;

    public static GoodsManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GoodsManager();

            return instance;
        }
    }

    private static int curGold;
    private static int curGem;
    private static int curEnergy;
    private static int maxEnergy;
    public int Gold { get { return curGold; } }
    public int Gem { get { return curGem; } }
    public int Energy { get { return curEnergy; } }
    public int MaxEnergy { get { return maxEnergy; } }

    private GoodsManager()
    {
        curGold = PlayerPrefs.HasKey("gold") ? PlayerPrefs.GetInt("gold") : 0;
        curGem = PlayerPrefs.HasKey("gem") ? PlayerPrefs.GetInt("gem") : 0;
        curEnergy = PlayerPrefs.HasKey("energy") ? PlayerPrefs.GetInt("energy") : 0;
    }

    public bool GoodsControl(int id, int val)
    {
        int temp = val;
        switch(id)
        {
            case 9001:
                temp += curGold;
                if (temp < 0 || temp > int.MaxValue)
                    return false;
                else
                {
                    curGold = temp;
                    SaveGoods();
                    return true;
                }
            case 9002:
                temp += curGem;
                if (temp < 0 || temp > int.MaxValue)
                    return false;
                else
                {
                    curGem = temp;
                    SaveGoods();
                    return true;
                }
            case 9003:
                temp += curEnergy;
                if (temp < 0 || temp > int.MaxValue)
                    return false;
                else
                {
                    curEnergy = temp;
                    SaveGoods();
                    return true;
                }
            default:
                return false;
        }
    }

    public bool ChargeEnergy(int val)
    {
        val += curEnergy;
        if (val < 0 || val > int.MaxValue)
            return false;
        else if(val > maxEnergy)
        {
            curEnergy = maxEnergy;
            return true;
        }
        else
        {
            curEnergy = val;
            SaveGoods();
            return true;
        }
    }

    public void SetMaxEnergy(int max)
    {
        maxEnergy = max;
    }

    public void SaveGoods()
    {
        PlayerPrefs.SetInt("gold", curGold);
        PlayerPrefs.SetInt("gem", curGem);
        PlayerPrefs.SetInt("energy", curEnergy);
    }
}