using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    public float speed;
    List<Vector3> directions;
    int dirIdx;

    GameObject enemy;
    Vector3 startPos;
    Vector3 dif;
    public GameManager manager;
    Vector3[] vecs;
    Vector3 spawnSpot;
    public ObjectManager objManager;

    void OnEnable()
    {
        dirIdx = 0;
    }

    void Update()
    {
        Move();
    }

    public void SetDirections(Vector3[] dir)
    {
        directions = new List<Vector3>();
        vecs = dir;

        foreach (Vector3 vec in dir)
        {
            directions.Add(vec);
        }
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, directions[dirIdx], speed * Time.deltaTime);

        dif = transform.position - startPos;
        if(dif.x > 0){
            transform.rotation = Quaternion.identity;
        }
        else if(dif.x < 0){
            transform.rotation = Quaternion.identity;
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if(dif.y > 0){
            transform.rotation = Quaternion.identity;
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if(dif.y < 0){
            transform.rotation = Quaternion.identity;
            transform.rotation = Quaternion.Euler(0, 0, 270);
        }

        if (transform.position == directions[dirIdx])
        {
            startPos = transform.position;
            dirIdx++;
        }
    }

    public void SetEnemyData(GameManager mng, GameObject enemy, Vector3 pos)
    {
        manager = mng;
        this.enemy = enemy;
        spawnSpot = pos;
        startPos = pos;
        transform.position = pos;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "HQ")
        {
            Enemy enemyLogic = enemy.GetComponent<Enemy>();
            switch (enemyLogic.enemyID)
            {
                case 90001:
                    GameObject melee = objManager.MakeObj("enemyMelee");
                    melee.transform.position = spawnSpot;
                    enemyLogic = melee.GetComponent<Enemy>();
                    break;
                case 90002:
                    GameObject range = objManager.MakeObj("enemyRange");
                    range.transform.position = spawnSpot;
                    enemyLogic = range.GetComponent<Enemy>();
                    break;
                case 90003:
                    GameObject flying = objManager.MakeObj("enemyFlying");
                    flying.transform.position = spawnSpot;
                    enemyLogic = flying.GetComponent<Enemy>();
                    break;
            }
            enemyLogic.SetHpUI();
            enemyLogic.SetDirections(vecs);
            gameObject.SetActive(false);
        }
    }
}