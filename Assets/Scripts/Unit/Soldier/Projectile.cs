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

    if (Vector2.Distance(transform.position, target.position) < 0.2f)
    {
        BaseUnit enemy = target.GetComponent<BaseUnit>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}



}
