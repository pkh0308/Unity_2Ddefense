using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Operator : MonoBehaviour
{
    public enum Type { Melee, Range};
    public enum SkillType { Active, Auto };
    public enum SkillRecoveryType { Attack, Auto };
    public Type type;
    public SkillType skillType;
    public SkillRecoveryType skillRecoveryType;

    public float health;
    public float maxHealth;
    public float sp;
    public float maxSp;

    protected float attackPower;
    public float AttackPower { get { return attackPower; } }
    protected float defensePower;
    public float DefensePower { get { return defensePower; } }
    float defenseRate;
    protected int spawnCost;
    public int SpawnCost { get { return spawnCost; } }

    public int canStop;
    int nowStop;
    public float fireRate;
    protected float curRate;
    public float attackRange;
    
    public int skillNum;
    public int operID;

    protected bool isAttacking;
    public bool isDied;
    public bool isFulled;

    public float skillRate;
    public float spRate;
    public float skillTime;
    public bool skillActivated;
    protected GameObject skillActivatedIcon;

    protected GameObject hpBar;
    protected GameObject spBar;
    Image hpLogic;
    Image spLogic;

    protected GameManager gameManager;
    protected ObjectManager objManager;
    public GameObject[] target;
    protected Animator anim;
    protected SpriteRenderer render;
    BoxCollider2D boxColl;

    List<GameObject> temp;

    void Awake()
    {
        anim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        boxColl = GetComponent<BoxCollider2D>();
        temp = new List<GameObject>();
        target = new GameObject[canStop];

        defenseRate = 100 / (100 + defensePower);
    }

    void OnEnable()
    {
        curRate = fireRate;
        health = maxHealth;
        sp = 0;

        isDied = false;
        isFulled = false;
        skillActivated = false;
        isAttacking = false;
            
        boxColl.enabled = true;
    }

    public void Initialize()
    {
        if (objManager == null)
            return;

        Vector3 hpPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(-0.5f, -0.4f, 0));
        Vector3 spPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(-0.5f, -0.5f, 0));
        hpBar = objManager.MakeObj("hpBar");
        spBar = objManager.MakeObj("spBar");
        hpBar.transform.position = hpPos;
        spBar.transform.position = spPos;
        hpLogic = hpBar.GetComponent<Image>();
        spLogic = spBar.GetComponent<Image>();

        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up);
        skillActivatedIcon.transform.position = pos;
        skillActivatedIcon.SetActive(false);
    }

    void OnDisable()
    {
        isDied = true;
        StopAllCoroutines();

        render.color = new Color(1, 1, 1, 1);

        for (int i = 0; i < target.Length; i++)
            target[i] = null;
    }

    public void SetManager(GameManager gm, ObjectManager om)
    {
        gameManager = gm;
        objManager = om;
    }

    public void SetSkillIcon(GameObject icon)
    {
        skillActivatedIcon = icon;
    }

    public void SetStatus(int atk, int def, int cost)
    {
        attackPower = atk;
        defensePower = def;
        spawnCost = cost;
    }

    public void OperUiOff(bool action)
    {
        hpBar.SetActive(action);
        spBar.SetActive(action);
        skillActivatedIcon.SetActive(action);
    }

    void LateUpdate()
    {
        UIUpdate();
    }

    void UIUpdate()
    {
        if(!isDied)
        {
            hpLogic.rectTransform.localScale = new Vector3(health / maxHealth, 1, 1);
            spLogic.rectTransform.localScale = new Vector3(sp / maxSp, 1, 1);
        }
    }

    // 적과 충돌 시 타겟으로 설정
    // 최대 저지 중일 경우 temp에 저장
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (type == Type.Range) return;

        switch (coll.gameObject.tag)
        {
            case "Enemy":
                for (int i = 0; i < target.Length; i++)
                    if (coll.gameObject == target[i])
                        return;

                Enemy enemyLogic = coll.gameObject.GetComponent<Enemy>();
                if (!enemyLogic.isDied)
                    SetTarget(coll.gameObject);

                if (isFulled)
                { 
                    if(!coll.gameObject.GetComponent<Enemy>().isDied)
                        temp.Add(coll.gameObject);
                }
                break;
        }
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (type == Type.Range) return;

        switch (coll.gameObject.tag)
        {
            case "Enemy":
                for (int i = 0; i < target.Length; i++)
                    if (coll.gameObject == target[i])
                        return;

                Enemy enemyLogic = coll.gameObject.GetComponent<Enemy>();
                if (!enemyLogic.isDied)
                    SetTarget(coll.gameObject);

                break;
        }
    }

    // 최대 저지 상태라 통과하는 경우 temp에 쌓여있는 타겟 제거
    void OnTriggerExit2D(Collider2D coll)
    {
        if (type == Type.Range) return;

        switch (coll.gameObject.tag)
        {
            case "Enemy":
                if(!isDied && temp.Count > 0) 
                    temp.RemoveAt(0);
                break;
        }
    }

    public void OnDamaged(float dmg)
    {
        dmg *= defenseRate;
        if (health > dmg)
        {
            health -= dmg;
        }
        else if (!isDied)
        {
            isDied = true;
            boxColl.enabled = false;
            StopAllCoroutines();
            TargetClear();
            StartCoroutine(OnDie());
        }
    }

    public void SetTarget(GameObject obj)
    {
        for (int i = 0; i < canStop; i++)
        {
            if (target[i] != null) continue;
            bool flyingEnemy = (obj.tag == "EnemyFlying");
            if (type == Type.Melee && flyingEnemy)
                continue;

            Enemy enemyLogic = obj.GetComponent<Enemy>();
            if (!enemyLogic.isBlocked)
            {
                target[i] = obj;
                if(!flyingEnemy) enemyLogic.isBlocked = true;
                break;
            }
        }
    }

    // 타겟이 사망한 경우 한칸씩 당겨옴
    // temp에 저장된 적이 있을 경우 타겟으로 당겨옴
    protected void TargetResearch()
    {
        for (int i = 0; i < canStop - 1; i++)
        {
            if(target[i] != null && !target[i].activeSelf)
            {
                target[i] = null;
            }
        }
        for (int i = 0; i < canStop - 1; i++)
        {
            if (target[i] == null && target[i+1] != null)
            {
                Enemy enemyLogic = target[i+1].GetComponent<Enemy>();
                if (!enemyLogic.isDied)
                {
                    target[i] = target[i+1];
                    target[i + 1] = null;
                    break;
                }
            }
        }

        if (temp.Count > 0 && target[canStop - 1] == null)
        {
            SetTarget(temp[0]);
            temp.RemoveAt(0);
        }
    }

    // 현재 저지 수 체크
    protected void FullCheck()
    {
        int count = 0;
        foreach(var obj in target)
            if (obj != null)
                count++;

        nowStop = count;
        isFulled = nowStop >= canStop ? true : false;
    }

    // 오퍼레이터 퇴각 시 호출
    // 타겟으로 삼은 적들의 block 상태 해제
    public void TargetClear()
    {
        for (int i = 0; i < canStop; i++)
        {
            if (target[i] != null)
            {
                Enemy enemyLogic = target[i].GetComponent<Enemy>();
                enemyLogic.isBlocked = false;
            }
        }
    }

    protected void SkillPoint()
    {
        if (skillRecoveryType == SkillRecoveryType.Attack || isDied)
            return;

        if (skillActivated)
        {
            sp -= maxSp / skillTime * Time.deltaTime;
            return;
        }

        if(sp < maxSp)
            sp += spRate * Time.deltaTime;

        if (sp >= maxSp && !skillActivatedIcon.activeSelf)
        {
            skillActivatedIcon.SetActive(true);
        }
    }

    IEnumerator OnDie()
    {
        anim.SetTrigger("doDie");
        gameManager.soundManager.PlaySfx(6, 0.8f);
        gameManager.OperCount(-1);
        gameManager.TileControl(transform.position, type);
        hpBar.SetActive(false);
        spBar.SetActive(false);
        if (skillActivatedIcon != null)
            skillActivatedIcon.SetActive(false);

        yield return new WaitForSeconds(2.9f);
        gameObject.SetActive(false);
    }
    
    protected void MouseClickCheck()
    {
        if (Input.GetMouseButtonDown(0) && !gameManager.operInfoSet.activeSelf && !gameManager.isPaused)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.up, 0.001f, LayerMask.GetMask("Operator"));
            if (hit && hit.collider.gameObject == gameObject)
            {
                gameManager.OperClick(this);
            }
        }           
    }

    public void skiilActive()
    {
        if (skillType == SkillType.Auto)
            return;

        StartCoroutine(Skill());
    }

    public virtual IEnumerator Skill()
    {
        yield return null;
    }
}