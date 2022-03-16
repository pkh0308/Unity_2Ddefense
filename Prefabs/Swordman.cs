using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordman : Operator
{
    void Update()
    {
        if (gameManager.CheckGameOvered()) return;

        if (target[0] != null && !isAttacking && !isDied)
            StartCoroutine(Attack());

        if (!isDied)
        {
            TargetSearch();
            FullCheck();
            MouseClickCheck();
        }     
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        if (curRate >= fireRate)
        {
            if (sp < maxSp)
            {
                anim.SetTrigger("doAttack");
                gameManager.soundManager.PlaySfx(0, 0.8f);
            }
            else
            {
                anim.SetTrigger("doSkill");
                gameManager.soundManager.PlaySfx(0, 1.2f);
            }
                

            Enemy enemyLogic = target[0].GetComponent<Enemy>();

            yield return new WaitForSeconds(0.3f);
            if (sp < maxSp)
            {
                enemyLogic.OnDamaged(attackPower);
                sp++;
            }
            else
            {
                enemyLogic.OnDamaged(attackPower * skillRate);
                sp = 0;
            }

            if (enemyLogic.isDied)
            {
                target[0] = null;
                TargetResearch();
            }

            yield return new WaitForSeconds(0.7f);
            curRate = 0;
            isAttacking = false;
        }
        else
        {
            curRate += Time.deltaTime;
            yield return null;
            isAttacking = false;
        }

        if (target[0] != null)
        {
            render.flipX = target[0].transform.position.x <= transform.position.x;
        }
        else
            render.flipX = true;
    }
}