using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    protected float health;
    public float maxHealth;
    public float attackPower;
    public float defensePower;
    float defenseRate;

    public enum enemyType { Melee, Range, Flying }
    public enemyType type;

    public float speed;
    public float fireRate;
    protected float curRate;
    public float attackRange;
    public int enemyID;

    public bool isAttacking;
    public bool isDied;
    public bool isBlocked;
 
    GameObject hpBar;
    Image hpLogic;

    public GameObject target;
    protected GameObject shade;
    protected Animator anim;
    protected GameManager gameManager;
    protected ObjectManager objManager;
    protected Rigidbody2D rigid;
    protected SpriteRenderer render;

    public List<Vector3> directions;
    public int dirIdx;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();

        defenseRate = 100 / (100 + defensePower);
    }

    void Update()
    {
        if (gameManager.CheckGameOvered()) return;

        if (target != null && !isAttacking && !isDied)
        {
            StartCoroutine(Attack());
        }
        Move();
    }

    void OnEnable()
    {
        curRate = fireRate;
        health = maxHealth;
        dirIdx = 0;

        isAttacking = false;
        isDied = false;
        isBlocked = false;
        if (shade != null)  shade.SetActive(true);
    }

    // enemy와 hp UI 연결
    // DirectionArrow 에서 호출해서 사용
    public void SetHpUI()
    {
        if (objManager != null)
        {
            hpBar = objManager.MakeObj("hpBar");
            hpLogic = hpBar.GetComponent<Image>();
        }
    }

    void OnDisable()
    {
        isDied = true;
        StopAllCoroutines();
        target = null;

        if (directions.Count > 0)
            directions.Clear();

        if (render != null)
            render.color = new Color(1, 1, 1, 1);
    }

    void LateUpdate()
    {
        UIUpdate();
    }

    void UIUpdate()
    {
        if(hpBar != null)
        {
            hpBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(-0.5f, -0.5f, 0));
            hpLogic.rectTransform.localScale = new Vector3(health / maxHealth, 1, 1);
        }
    }

    public void SetDirections(Vector3[] dir)
    {
        directions = new List<Vector3>();

        foreach (Vector3 vec in dir)
        {
            directions.Add(vec);
        }
    }

    public void SetManager(GameManager gm, ObjectManager om)
    {
        gameManager = gm;
        objManager = om;
    }

    protected void Move()
    {
        if (isAttacking || isDied || isBlocked)
        {
            anim.SetBool("isMoving", false);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, directions[dirIdx], speed * Time.deltaTime);

        if (transform.position == directions[dirIdx])
            dirIdx++;

        anim.SetBool("isMoving", true);
        render.flipX = true;
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        rigid.velocity = Vector3.zero;
        if (target != null)
        {
            render.flipX = target.transform.position.x >= transform.position.x;
        }

        if (curRate >= fireRate)
        {
            anim.SetTrigger("doAttack");
            Operator operatorLogic = target.GetComponent<Operator>();

            yield return new WaitForSeconds(0.3f);
            operatorLogic.OnDamaged(attackPower);
            TargetCheck();

            yield return new WaitForSeconds(0.7f);
            curRate = 0;
            isAttacking = false;
        }
        else
        {
            curRate += Time.deltaTime;
            yield return null;
            isAttacking = false;
            TargetCheck();
        }
    }

    public void OnDamaged(float dmg)
    {
        dmg *= defenseRate;
        if (health > dmg)
        {
            health -= dmg;
        }
        else if(!isDied && gameObject.activeSelf)
        {
            isDied = true;
            StopAllCoroutines();
            StartCoroutine(OnDie());
        }
    }

    protected void TargetSearch()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Operator"));
        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                Operator operLogic = cols[i].gameObject.GetComponent<Operator>();
                if (!operLogic.isDied)
                    SetTarget(cols[i].gameObject);
            }
        }
        else
        {
            target = null;
            isAttacking = false;
            Debug.Log("타겟 해제");
        }
    }

    protected void TargetCheck()
    {
        if (target != null)
        {
            Operator operLogic = target.GetComponent<Operator>();
            if(operLogic.isDied)
            {
                StopCoroutine(Attack());
                isAttacking = false;
                isBlocked = false;
                target = null;
            }
        }
    }

    IEnumerator OnDie()
    {
        anim.SetTrigger("doDie");
        gameManager.soundManager.PlaySfx(6, 0.8f);
        hpBar.SetActive(false);

        yield return new WaitForSeconds(3.0f);
        gameObject.SetActive(false);
        gameManager.EnemyCntCheck();
    }

    public void SetTarget(GameObject obj)
    {
        Operator operLogic = obj.GetComponent<Operator>();
        if (target == null && !operLogic.isFulled && !operLogic.isDied)
            target = obj;
        else
            return;
    }

    public void Knuckback(float dmg, Vector3 dirVec, float offset)
    {
        StartCoroutine(_Knuckback(dmg, dirVec, offset));
    }

    IEnumerator _Knuckback(float dmg, Vector3 dirVec, float offset)
    {
        OnDamaged(dmg);
        StopCoroutine(Attack());
        if (!isDied)
        {
            transform.position += dirVec * offset;
            target = null;
            isAttacking = false;

            yield return new WaitForSeconds(0.8f);
            isBlocked = false;
        }
    }

    protected void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "HQ")
        {
            isDied = true;
            gameManager.EnemyEntrance();
            gameObject.SetActive(false);
            hpBar.SetActive(false);
        }
        else if (coll.gameObject.tag == "Operator")
        {
            OnEncounter(coll);
        }
    }

    protected void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Operator")
        {
            OnEncounter(coll);
        }
    }

    // 오퍼레이터와 충돌 시 처리
    // Melee 타입일 경우 충돌 대상을 타겟 설정(조건 검사는 SetTarget에서 실행)
    // 자신이 block 상태가 아니고, 타겟 오퍼가 최대 저지중이 아니라면 자신을 block
    protected void OnEncounter(Collider2D coll)
    {
        if (type != enemyType.Melee) return;

        SetTarget(coll.gameObject);
        Operator operLogic = coll.GetComponent<Operator>();
        if (!isBlocked && !operLogic.isFulled)
        {
            isBlocked = true;
        }
    }

    // 그림자 개체 연결
    public void SetShade(GameObject obj)
    {
        shade = obj;
        shade.transform.position = transform.position + Vector3.down * 0.4f;
        shade.transform.SetParent(transform);
    }
}