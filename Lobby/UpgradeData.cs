using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeData : MonoBehaviour
{
    public int operId;
    public enum UpagradeCategory { Atk, Def, Cost }
    public UpagradeCategory category;
    public int curLevel;
    public int maxLevel;

    public int costId;
    public int costVal;
}