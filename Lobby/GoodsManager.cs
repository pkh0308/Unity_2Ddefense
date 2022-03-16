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
    public int Gold { get { return curGold; } }
    public int Gem { get { return curGem; } }
    public int Energy { get { return curEnergy; } }

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

    public void SaveGoods()
    {
        PlayerPrefs.SetInt("gold", curGold);
        PlayerPrefs.SetInt("gem", curGem);
        PlayerPrefs.SetInt("energy", curEnergy);
    }
}