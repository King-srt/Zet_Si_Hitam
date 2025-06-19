using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
    private int damage;

public void Init(Transform targetTransform, int dmg)
{
    target = targetTransform;
    damage = dmg;
}

void Update()
{
    if (target == null)
    {
        Destroy(gameObject);
        return;
    }

    Vector3 dir = (target.position - transform.position).normalized;
    transform.position += dir * speed * Time.deltaTime;
    
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            BaseEnemy enemy = target.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
}



}
