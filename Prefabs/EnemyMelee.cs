using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : Enemy
{
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
}