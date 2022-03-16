using System.IO;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameManager gameManager;
    int operIdx;
    int operStatusIndex;
    public OperDrag[] operSets;

    public GameObject enemyMeleePrefab;
    public GameObject enemyRangePrefab;
    public GameObject enemyBulletAPrefab;
    public GameObject swordmanPrefab;
    public GameObject archerPrefab;
    public GameObject playerArrowPrefab;
    public GameObject casterPrefab;
    public GameObject playerFireBallPrefab;
    public GameObject defenderPrefab;
    public GameObject knuckbackShotPrefab;

    public GameObject directionArrowPrefab;
    public GameObject hpBarPrefab;
    public GameObject spBarPrefab;
    public GameObject swordmanIconPrefab;
    public GameObject archerIconPrefab;
    public GameObject casterIconPrefab;
    public GameObject defenderIconPrefab;
    public GameObject skillActivatedIconPrefab;
    public GameObject attackRangePrefab;

    GameObject[] enemyMelee;
    GameObject[] enemyRange;
    GameObject[] enemyBulletA;
    GameObject[] swordman;
    GameObject[] archer;
    GameObject[] playerArrow;
    GameObject[] caster;
    GameObject[] playerFireBall;
    GameObject[] defender;
    GameObject[] knuckbackShot;

    GameObject[] directionArrow;
    GameObject[] hpBar;
    GameObject[] spBar;
    GameObject[] swordmanIcon;
    GameObject[] archerIcon;
    GameObject[] casterIcon;
    GameObject[] defenderIcon;
    GameObject[] skillActivatedIcon;
    GameObject[] attackRange;

    int[] status;
    GameObject[] targetPool;

    // Start is called before the first frame update
    void Awake()
    {
        enemyMelee = new GameObject[30];
        enemyRange = new GameObject[30];
        enemyBulletA = new GameObject[50];
        swordman = new GameObject[10];
        archer = new GameObject[10];
        playerArrow = new GameObject[50];
        caster = new GameObject[10];
        playerFireBall = new GameObject[50];
        defender = new GameObject[10];
        knuckbackShot = new GameObject[50];

        directionArrow = new GameObject[30];
        hpBar = new GameObject[50];
        spBar = new GameObject[50];
        swordmanIcon = new GameObject[10];
        archerIcon = new GameObject[10];
        casterIcon = new GameObject[10];
        defenderIcon = new GameObject[10];
        skillActivatedIcon = new GameObject[50];
        attackRange = new GameObject[10];

        status = new int[12];
        Generate();
    }

    void Generate()
    {
        TextAsset operStatus = Resources.Load("operatorStatus") as TextAsset;
        StringReader reader = new StringReader(operStatus.text);

        int index = 0;
        string name = "";
        while(reader != null)
        {
            string line = reader.ReadLine();
            if (line == null) break;
            
            switch (line)
            {
                case "10001":
                    name = "swordman";
                    break;
                case "20001":
                    name = "archer";
                    break;
                case "30001":
                    name = "caster";
                    break;
                case "40001":
                    name = "defender";
                    break;
                default:
                    string[] atkData = line.Split(',');
                    int atk = int.Parse(atkData[PlayerPrefs.GetInt(name + "Atk")]);
                    status[index] = atk;

                    line = reader.ReadLine();
                    string[] defData = line.Split(',');
                    int def = int.Parse(defData[PlayerPrefs.GetInt(name + "Def")]);
                    status[index + 1] = def;

                    line = reader.ReadLine();
                    string[] costData = line.Split(',');
                    int cost = int.Parse(costData[PlayerPrefs.GetInt(name + "Cost")]);
                    status[index + 2] = cost;

                    index += 3;
                    break;
            }
        }
        reader.Close();

        //UI
        for (int idx = 0; idx < directionArrow.Length; idx++)
        {
            directionArrow[idx] = Instantiate(directionArrowPrefab);
            directionArrow[idx].SetActive(false);
        }
        for (int idx = 0; idx < hpBar.Length; idx++)
        {
            hpBar[idx] = Instantiate(hpBarPrefab, GameObject.Find("MinorCanvas").transform);
            hpBar[idx].gameObject.SetActive(false);
        }
        for (int idx = 0; idx < spBar.Length; idx++)
        {
            spBar[idx] = Instantiate(spBarPrefab, GameObject.Find("MinorCanvas").transform);
            spBar[idx].gameObject.SetActive(false);
        }
        for (int idx = 0; idx < swordmanIcon.Length; idx++)
        {
            swordmanIcon[idx] = Instantiate(swordmanIconPrefab, GameObject.Find("MajorCanvas").transform);
            swordmanIcon[idx].gameObject.SetActive(false);
        }
        for (int idx = 0; idx < archerIcon.Length; idx++)
        {
            archerIcon[idx] = Instantiate(archerIconPrefab, GameObject.Find("MajorCanvas").transform);
            archerIcon[idx].gameObject.SetActive(false);
        }
        for (int idx = 0; idx < casterIcon.Length; idx++)
        {
            casterIcon[idx] = Instantiate(casterIconPrefab, GameObject.Find("MajorCanvas").transform);
            casterIcon[idx].gameObject.SetActive(false);
        }
        for (int idx = 0; idx < defenderIcon.Length; idx++)
        {
            defenderIcon[idx] = Instantiate(defenderIconPrefab, GameObject.Find("MajorCanvas").transform);
            defenderIcon[idx].gameObject.SetActive(false);
        }
        for (int idx = 0; idx < skillActivatedIcon.Length; idx++)
        {
            skillActivatedIcon[idx] = Instantiate(skillActivatedIconPrefab, GameObject.Find("MinorCanvas").transform);
            skillActivatedIcon[idx].gameObject.SetActive(false);
        }
        for (int idx = 0; idx < attackRange.Length; idx++)
        {
            attackRange[idx] = Instantiate(attackRangePrefab);
            attackRange[idx].gameObject.SetActive(false);
        }

        // emeny
        for (int idx = 0; idx < enemyMelee.Length; idx++)
        {
            enemyMelee[idx] = Instantiate(enemyMeleePrefab);
            enemyMelee[idx].SetActive(false);
            Enemy enemyLogic = enemyMelee[idx].GetComponent<Enemy>();
            enemyLogic.SetManager(gameManager, this);
        }
        for (int idx = 0; idx < enemyRange.Length; idx++)
        {
            enemyRange[idx] = Instantiate(enemyRangePrefab);
            enemyRange[idx].SetActive(false);
            Enemy enemyLogic = enemyRange[idx].GetComponent<Enemy>();
            enemyLogic.SetManager(gameManager, this);
        }
        for (int idx = 0; idx < enemyBulletA.Length; idx++)
        {
            enemyBulletA[idx] = Instantiate(enemyBulletAPrefab);
            enemyBulletA[idx].gameObject.SetActive(false);
        }

        // oper
        for (int idx = 0; idx < swordman.Length; idx++)
        {
            swordman[idx] = Instantiate(swordmanPrefab);
            swordman[idx].SetActive(false);
            Operator operLogic = swordman[idx].GetComponent<Operator>();
            operLogic.SetManager(gameManager, this);
            operLogic.SetSkillIcon(skillActivatedIcon[idx]);
            operLogic.SetStatus(status[operStatusIndex], status[operStatusIndex + 1], status[operStatusIndex + 2]);
        }
        operIdx += swordman.Length;
        operStatusIndex += 3;

        for (int idx = 0; idx < archer.Length; idx++)
        {
            archer[idx] = Instantiate(archerPrefab);
            archer[idx].SetActive(false);
            Operator operLogic = archer[idx].GetComponent<Operator>();
            operLogic.SetManager(gameManager, this);
            operLogic.SetSkillIcon(skillActivatedIcon[operIdx + idx]);
            operLogic.SetStatus(status[operStatusIndex], status[operStatusIndex + 1], status[operStatusIndex + 2]);
        }
        operIdx += archer.Length;
        operStatusIndex += 3;
        for (int idx = 0; idx < playerArrow.Length; idx++)
        {
            playerArrow[idx] = Instantiate(playerArrowPrefab);
            playerArrow[idx].gameObject.SetActive(false);
        }

        for (int idx = 0; idx < caster.Length; idx++)
        {
            caster[idx] = Instantiate(casterPrefab);
            caster[idx].SetActive(false);
            Operator operLogic = caster[idx].GetComponent<Operator>();
            operLogic.SetManager(gameManager, this);
            operLogic.SetSkillIcon(skillActivatedIcon[operIdx + idx]);
            operLogic.SetStatus(status[operStatusIndex], status[operStatusIndex + 1], status[operStatusIndex + 2]);
        }
        operIdx += caster.Length;
        operStatusIndex += 3;
        for (int idx = 0; idx < playerFireBall.Length; idx++)
        {
            playerFireBall[idx] = Instantiate(playerFireBallPrefab);
            playerFireBall[idx].gameObject.SetActive(false);
        }

        for (int idx = 0; idx < defender.Length; idx++)
        {
            defender[idx] = Instantiate(defenderPrefab);
            defender[idx].SetActive(false);
            Operator operLogic = defender[idx].GetComponent<Operator>();
            operLogic.SetManager(gameManager, this);
            operLogic.SetSkillIcon(skillActivatedIcon[operIdx + idx]);
            operLogic.SetStatus(status[operStatusIndex], status[operStatusIndex + 1], status[operStatusIndex + 2]);
        }
        operIdx += defender.Length;
        operStatusIndex += 3;
        for (int idx = 0; idx < knuckbackShot.Length; idx++)
        {
            knuckbackShot[idx] = Instantiate(knuckbackShotPrefab);
            knuckbackShot[idx].gameObject.SetActive(false);
        }

        for (int i = 0; i < operSets.Length; i++)
            operSets[i].Initialize();
    }

    public GameObject MakeObj(string type)
    {
        switch (type)
        {
            case "enemyMelee":
                targetPool = enemyMelee;
                break;
            case "enemyRange":
                targetPool = enemyRange;
                break;
            case "swordman":
                targetPool = swordman;
                break;
            case "archer":
                targetPool = archer;
                break;
            case "caster":
                targetPool = caster;
                break;
            case "defender":
                targetPool = defender;
                break;
            case "directionArrow":
                targetPool = directionArrow;
                break;
            case "hpBar":
                targetPool = hpBar;
                break;
            case "spBar":
                targetPool = spBar;
                break;
            case "swordmanIcon":
                targetPool = swordmanIcon;
                break;
            case "archerIcon":
                targetPool = archerIcon;
                break;
            case "casterIcon":
                targetPool = casterIcon;
                break;
            case "defenderIcon":
                targetPool = defenderIcon;
                break;
            case "skillActivatedIcon":
                targetPool = skillActivatedIcon;
                break;
            case "attackRange":
                targetPool = attackRange;
                break;
            case "playerArrow":
                targetPool = playerArrow;
                break;
            case "playerFireBall":
                targetPool = playerFireBall;
                break;
            case "enemyBulletA":
                targetPool = enemyBulletA;
                break;
            case "knuckbackShot":
                targetPool = knuckbackShot;
                break;
        }

        for (int idx = 0; idx < targetPool.Length; idx++)
        {
            if (!targetPool[idx].activeSelf)
            {
                targetPool[idx].SetActive(true);
                return targetPool[idx];
            }
        }
        return null;
    }

    public GameObject[] GetPool(string type)
    {
        switch (type)
        {
            case "enemyMelee":
                targetPool = enemyMelee;
                break;
            case "enemyRange":
                targetPool = enemyRange;
                break;
            case "swordman":
                targetPool = swordman;
                break;
            case "archer":
                targetPool = archer;
                break;
            case "caster":
                targetPool = caster;
                break;
            case "defender":
                targetPool = defender;
                break;
            case "directionArrow":
                targetPool = directionArrow;
                break;
            case "hpBar":
                targetPool = hpBar;
                break;
            case "spBar":
                targetPool = spBar;
                break;
            case "swordmanIcon":
                targetPool = swordmanIcon;
                break;
            case "archerIcon":
                targetPool = archerIcon;
                break;
            case "casterIcon":
                targetPool = casterIcon;
                break;
            case "defenderIcon":
                targetPool = defenderIcon;
                break;
            case "skillActivatedIcon":
                targetPool = skillActivatedIcon;
                break;
            case "attackRange":
                targetPool = attackRange;
                break;
            case "playerArrow":
                targetPool = playerArrow;
                break;
            case "playerFireBall":
                targetPool = playerFireBall;
                break;
            case "enemyBulletA":
                targetPool = enemyBulletA;
                break;
            case "knuckbackShot":
                targetPool = knuckbackShot;
                break;
        }
        return targetPool;
    }

    public GameObject GetOne(string type)
    {
        switch (type)
        {
            case "enemyMelee":
                targetPool = enemyMelee;
                break;
            case "enemyRange":
                targetPool = enemyRange;
                break;
            case "swordman":
                targetPool = swordman;
                break;
            case "archer":
                targetPool = archer;
                break;
            case "caster":
                targetPool = caster;
                break;
            case "defender":
                targetPool = defender;
                break;
            case "directionArrow":
                targetPool = directionArrow;
                break;
            case "hpBar":
                targetPool = hpBar;
                break;
            case "spBar":
                targetPool = spBar;
                break;
            case "swordmanIcon":
                targetPool = swordmanIcon;
                break;
            case "archerIcon":
                targetPool = archerIcon;
                break;
            case "casterIcon":
                targetPool = casterIcon;
                break;
            case "defenderIcon":
                targetPool = defenderIcon;
                break;
            case "skillActivatedIcon":
                targetPool = skillActivatedIcon;
                break;
            case "attackRange":
                targetPool = attackRange;
                break;
            case "playerArrow":
                targetPool = playerArrow;
                break;
            case "playerFireBall":
                targetPool = playerFireBall;
                break;
            case "enemyBulletA":
                targetPool = enemyBulletA;
                break;
            case "knuckbackShot":
                targetPool = knuckbackShot;
                break;
        }
        return targetPool[0];
    }

    public int GetActiveCount(string type)
    {
        int count = 0;

        switch (type)
        {
            case "enemyMelee":
                targetPool = enemyMelee;
                break;
            case "enemyRange":
                targetPool = enemyRange;
                break;
            case "swordman":
                targetPool = swordman;
                break;
            case "archer":
                targetPool = archer;
                break;
            case "caster":
                targetPool = caster;
                break;
            case "defender":
                targetPool = defender;
                break;
            case "directionArrow":
                targetPool = directionArrow;
                break;
            case "hpBar":
                targetPool = hpBar;
                break;
            case "spBar":
                targetPool = spBar;
                break;
            case "swordmanIcon":
                targetPool = swordmanIcon;
                break;
            case "archerIcon":
                targetPool = archerIcon;
                break;
            case "casterIcon":
                targetPool = casterIcon;
                break;
            case "defenderIcon":
                targetPool = defenderIcon;
                break;
            case "skillActivatedIcon":
                targetPool = skillActivatedIcon;
                break;
            case "attackRange":
                targetPool = attackRange;
                break;
            case "playerArrow":
                targetPool = playerArrow;
                break;
            case "playerFireBall":
                targetPool = playerFireBall;
                break;
            case "enemyBulletA":
                targetPool = enemyBulletA;
                break;
            case "knuckbackShot":
                targetPool = knuckbackShot;
                break;
        }

        for (int idx = 0; idx < targetPool.Length; idx++)
        {
            if (targetPool[idx].activeSelf)
            {
                count++;
            }
        }
        return count;
    }

    public void AllStop()
    {
        for (int idx = 0; idx < enemyMelee.Length; idx++)
        {
            if (enemyMelee[idx].activeSelf)
            {
                Animator anim = enemyMelee[idx].GetComponent<Animator>();
                anim.speed = 0.0f;
                Enemy enemyLogic = enemyMelee[idx].GetComponent<Enemy>();
                enemyLogic.isBlocked = true;
            }
        }

        for (int idx = 0; idx < enemyRange.Length; idx++)
        {
            if (enemyRange[idx].activeSelf)
            {
                Animator anim = enemyRange[idx].GetComponent<Animator>();
                anim.speed = 0.0f;
                Enemy enemyLogic = enemyRange[idx].GetComponent<Enemy>();
                enemyLogic.isBlocked = true;
            }
        }

        for (int idx = 0; idx < swordman.Length; idx++)
        {
            if (swordman[idx].activeSelf)
            {
                Animator anim = swordman[idx].GetComponent<Animator>();
                anim.speed = 0.0f;
            }
        }

        for (int idx = 0; idx < archer.Length; idx++)
        {
            if (archer[idx].activeSelf)
            {
                Animator anim = archer[idx].GetComponent<Animator>();
                anim.speed = 0.0f;
            }
        }

        for (int idx = 0; idx < caster.Length; idx++)
        {
            if (caster[idx].activeSelf)
            {
                Animator anim = caster[idx].GetComponent<Animator>();
                anim.speed = 0.0f;
            }
        }

        for (int idx = 0; idx < defender.Length; idx++)
        {
            if (defender[idx].activeSelf)
            {
                Animator anim = defender[idx].GetComponent<Animator>();
                anim.speed = 0.0f;
            }
        }
    }
}