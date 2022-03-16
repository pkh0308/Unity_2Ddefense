using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    public GameObject hq;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hq.tag == "Operator" && other.tag == "Enemy")
        {
            Operator operatorLogic = hq.GetComponent<Operator>();
            operatorLogic.SetTarget(other.gameObject);
        }
        else if (hq.tag == "Enemy" && other.tag == "Operator")
        {
            Enemy enemyLogic = hq.GetComponent<Enemy>();
            enemyLogic.SetTarget(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (hq.tag == "Enemy" && other.tag == "Operator")
        {
            Enemy enemyLogic = hq.GetComponent<Enemy>();
            enemyLogic.SetTarget(null);
        }
    }
}