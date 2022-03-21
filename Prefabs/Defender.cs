using System.Collections;
using UnityEngine;

public class Defender : Operator
{
    void Update()
    {
        if (gameManager.CheckGameOvered()) return;

        if (target[0] != null && !isAttacking && !isDied && !skillActivated)
            StartCoroutine(Attack());

        if (!isDied)
        {
            FullCheck();
            SkillPoint();
            MouseClickCheck();
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        if (curRate >= fireRate)
        {
            anim.SetTrigger("doAttack");
            gameManager.soundManager.PlaySfx(3, 0.8f);
            Enemy enemyLogic = target[0].GetComponent<Enemy>();

            yield return new WaitForSeconds(0.3f);
            enemyLogic.OnDamaged(attackPower);
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

    public override IEnumerator Skill()
    {
        skillActivated = true;
        StopCoroutine(Attack());
        anim.SetTrigger("doSkill");
        gameManager.soundManager.PlaySfx(3, 1.4f);
        skillActivatedIcon.gameObject.SetActive(false);
        GameObject knuckbackL = objManager.MakeObj("knuckbackShot");
        knuckbackL.GetComponent<KnuckbackShot>().SetTarget(transform.position, Vector2.left, attackPower * skillRate);
        GameObject knuckbackR = objManager.MakeObj("knuckbackShot");
        knuckbackR.GetComponent<KnuckbackShot>().SetTarget(transform.position, Vector2.right, attackPower * skillRate);
        GameObject knuckbackU = objManager.MakeObj("knuckbackShot");
        knuckbackU.GetComponent<KnuckbackShot>().SetTarget(transform.position, Vector2.up, attackPower * skillRate);
        GameObject knuckbackD = objManager.MakeObj("knuckbackShot");
        knuckbackD.GetComponent<KnuckbackShot>().SetTarget(transform.position, Vector2.down, attackPower * skillRate);

        yield return new WaitForSeconds(0.4f);
        skillActivated = false;
    }

    protected new void SkillPoint()
    {
        if (skillRecoveryType == SkillRecoveryType.Attack || isDied)
            return;

        if (skillActivated)
        {
            sp = 0;
            return;
        }

        if (sp < maxSp)
            sp += spRate * Time.deltaTime;

        if (sp >= maxSp && !skillActivatedIcon.activeSelf)
        {
            skillActivatedIcon.SetActive(true);
        }
    }
}