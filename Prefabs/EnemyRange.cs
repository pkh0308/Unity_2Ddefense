using System.Collections;
using UnityEngine;

public class EnemyRange : Enemy
{
    void Update()
    {
        if (gameManager.CheckGameOvered()) return;

        if (target != null && !isAttacking && !isDied)
        {
            StartCoroutine(Attack());
        }

        Move();
        TargetSearch();
        DistanceCheck();
    }

    IEnumerator Attack()
    {
        rigid.velocity = Vector3.zero;
        if (target != null)
        {
            render.flipX = target.transform.position.x >= transform.position.x;
        }

        if (curRate >= fireRate)
        {
            isAttacking = true;
            anim.SetTrigger("doAttack");

            yield return new WaitForSeconds(0.3f);
            GameObject fireBall = objManager.MakeObj("enemyBulletA");
            fireBall.transform.position = transform.position;
            fireBall.GetComponent<Bullet>().SetTarget(target, attackPower);
            TargetCheck();

            yield return new WaitForSeconds(0.7f);
            curRate = 0;
            isAttacking = false;
        }
        else
        {
            curRate += Time.deltaTime;
            yield return null;
            TargetCheck();
        }
    }

    new void TargetCheck()
    {
        if (target != null)
        {
            Operator operLogic = target.GetComponent<Operator>();
            if (operLogic.isDied)
            {
                StopCoroutine(Attack());
                isAttacking = false;
                target = null;
            }
        }
    }

    new void TargetSearch()
    {
        if (target != null)
            return;

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Operator"));
        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                SetTarget(cols[i].gameObject);
            }
        }
    }

    new public void SetTarget(GameObject obj)
    {
        Operator operLogic = obj.GetComponent<Operator>();
        if (target == null && !operLogic.isDied)
            target = obj;
        else
            return;
    }

    void DistanceCheck()
    {
        if (target == null)
            return;

        float dis = Vector3.Distance(transform.position, target.transform.position);
        if (dis > attackRange)
        {
            target = null;
        }
    }

}