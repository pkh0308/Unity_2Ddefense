using UnityEngine;

public class Bullet : MonoBehaviour
{
    GameObject _target;
    Enemy enemyLogic;
    Operator operLogic;
    public float arrowSpeed;
    float damage;
    SpriteRenderer render;
    public enum BulletType { playerArrow, playerFireBall, EnemyBulletA }
    public BulletType bulletType;

    void OnEnable()
    {
        _target = null;
        if (render == null)
            render = GetComponent<SpriteRenderer>();
        else
            render.flipX = false;
    }

    void OnDisable()
    {
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if(_target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, arrowSpeed * Time.deltaTime);
        }
    }

    public void SetTarget(GameObject target, float attackPower)
    {
        _target = target;
        if(bulletType == BulletType.EnemyBulletA)
            operLogic = _target.GetComponent<Operator>();
        else
            enemyLogic = _target.GetComponent<Enemy>();
        damage = attackPower;

        int offset = transform.position.y < _target.transform.position.y ? 1 : -1;
        float angle = GetAngle(transform.position, _target.transform.position);
        if (bulletType != BulletType.playerArrow)
            angle -= 90;
        if (transform.position.x > _target.transform.position.x)
        {
            render.flipX = true;
            offset *= -1;
        }
        Vector3 rotationVec = Vector3.forward * angle * offset;
        transform.Rotate(rotationVec);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject == _target)
        {
            if (bulletType == BulletType.EnemyBulletA)
                operLogic.OnDamaged(damage);
            else
                enemyLogic.OnDamaged(damage);

            gameObject.SetActive(false);
        }
    }

    float GetAngle(Vector3 pos1, Vector3 pos2)
    {
        float x = Mathf.Abs(pos1.x - pos2.x);
        float y = Mathf.Abs(pos1.y - pos2.y);
        if (x == 0) x = 0.01f;
        return Mathf.Atan(y / x) * Mathf.Rad2Deg;
    }
}