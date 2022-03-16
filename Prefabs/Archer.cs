using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Archer : Operator
{
    void Update()
    {
        if (gameManager.CheckGameOvered()) return;

        if (target[0] != null && !isAttacking && !isDied)
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

            if(skillActivated)
            {
                anim.SetTrigger("doSkillAttack");
                gameManager.soundManager.PlaySfx(1, 0.8f);
            }   
            else
            {
                anim.SetTrigger("doAttack");
                gameManager.soundManager.PlaySfx(1, 1.2f);
            }  
            GameObject arrow = objManager.MakeObj("playerArrow");
            arrow.transform.position = transform.position;
            arrow.GetComponent<Bullet>().SetTarget(target[0], attackPower);
            Enemy enemyLogic = target[0].GetComponent<Enemy>();

            yield return new WaitForSeconds(1.0f);
            if (enemyLogic.isDied)
            {
                target[0] = null;
                TargetResearch();
                arrow.SetActive(false);
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

    new protected void TargetSearch()
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
        if(dis > attackRange)
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
        skillActivated = true;
        anim.SetBool("isUsingSkill", true);
        skillActivatedIcon.gameObject.SetActive(false);
        float temp = fireRate;
        fireRate *= (1 - skillRate);

        yield return new WaitForSeconds(skillTime);
        fireRate = temp;
        skillActivated = false;
        anim.SetBool("isUsingSkill", false);
    }
}