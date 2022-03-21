using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caster : Operator
{
    void Update()
    {
        if (gameManager.CheckGameOvered()) return;

        if (target[0] != null && !isAttacking && !isDied && !skillActivated)
            StartCoroutine(Attack());

        if (!isDied)
        {
            TargetSearch();
            DistanceCheck();
            SkillPoint();
            MouseClickCheck();
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        if (curRate >= fireRate)
        {
            if (target[0] != null)
                render.flipX = target[0].transform.position.x <= transform.position.x;
            else
                render.flipX = true;

            anim.SetTrigger("doAttack");
            gameManager.soundManager.PlaySfx(2);
            GameObject fireBall = objManager.MakeObj("playerFireBall");
            fireBall.transform.position = transform.position;
            fireBall.GetComponent<Bullet>().SetTarget(target[0], attackPower);
            Enemy enemyLogic = target[0].GetComponent<Enemy>();

            yield return new WaitForSeconds(1.0f);
            if (enemyLogic.isDied)
            {
                target[0] = null;
                TargetResearch();
                fireBall.SetActive(false);
            }
            curRate = 0;
            isAttacking = false;
        }
        else
        {
            curRate += Time.deltaTime;
            yield return null;
            isAttacking = false;
        }

        if (target[0] == null)
        {
            render.flipX = true;
        }
    }

    protected void TargetSearch()
    { 
        if (target[0] != null)
            return;

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy"));
        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                Enemy enemyLogic = cols[i].gameObject.GetComponent<Enemy>();
                if (!enemyLogic.isDied)
                    SetTarget(cols[i].gameObject);
            }
        }
    }

    void DistanceCheck()
    {
        if (target[0] == null)
            return;

        float dis = Vector3.Distance(transform.position, target[0].transform.position);
        if (dis > attackRange)
        {
            target[0] = null;
        }
    }

    new public void SetTarget(GameObject obj)
    {
        if (target[0] == null)
        {
            target[0] = obj;
        }
    }

    public override IEnumerator Skill()
    {
        float timeCount = 0;
        skillActivated = true;
        anim.SetBool("isUsingSkill", true);
        skillActivatedIcon.gameObject.SetActive(false);

        while(timeCount < skillTime)
        {
            yield return new WaitForSeconds(0.7f);
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy"));
            if (cols.Length > 0)
            {
                gameManager.soundManager.PlaySfx(2);
                for (int i = 0; i < cols.Length; i++)
                {
                    Enemy enemyLogic = cols[i].gameObject.GetComponent<Enemy>();
                    if (!enemyLogic.isDied)
                    {
                        GameObject fireBall = objManager.MakeObj("playerFireBall");
                        fireBall.transform.position = transform.position;
                        fireBall.GetComponent<Bullet>().SetTarget(cols[i].gameObject, attackPower * skillRate);
                    }
                }
            }
            timeCount += 0.7f;
        }
        skillActivated = false;
        anim.SetBool("isUsingSkill", false);
    }
}