using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnuckbackShot : MonoBehaviour
{
    Rigidbody2D rigid;
    Vector3 targetPos;
    Vector2 dirVec;
    public float knuckbackOffset;
    public float movingOffset;
    public float speed;
    float damage;
    int dir;

    void OnEnable()
    {
        if(rigid == null)
            rigid = GetComponent<Rigidbody2D>();
    }

    void OnDisable()
    {
        targetPos = Vector3.zero;
    }

    void Update()
    {
        if (targetPos == Vector3.zero)  return;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (transform.position == targetPos)
            gameObject.SetActive(false);
    }

    public void SetTarget(Vector3 pos, Vector2 dir, float dmg)
    {
        transform.position = pos;
        dirVec = dir;
        targetPos = pos + (Vector3)dirVec * movingOffset;
        damage = dmg;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            Enemy enemyLogic = coll.gameObject.GetComponent<Enemy>();
            enemyLogic.Knuckback(damage, dirVec, knuckbackOffset);
        }
    }
}