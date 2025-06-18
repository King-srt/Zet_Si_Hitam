using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    public Transform target;
    public float jumpDuration = 0.5f;
    public float jumpHeight = 1f;

    private Vector3 startPoint;
    private float timer;

    void Start()
    {
        startPoint = transform.position;
    }

    void Update()
    {
        if (target == null) return;

        timer += Time.deltaTime;
        float t = timer / jumpDuration;

        // Curve untuk loncat
        Vector3 midPoint = (startPoint + target.position) / 2 + Vector3.up * jumpHeight;
        Vector3 position = Vector3.Lerp(
            Vector3.Lerp(startPoint, midPoint, t),
            Vector3.Lerp(midPoint, target.position, t),
            t
        );

        transform.position = position;

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
