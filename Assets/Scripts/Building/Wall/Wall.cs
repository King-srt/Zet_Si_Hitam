using UnityEngine;
using System.Collections.Generic;

public class Wall : BaseBuilding
{
    public float connectionRadius = 1.1f;
    public LayerMask wallLayer;

    private SpriteRenderer spriteRenderer;

    public Sprite soloSprite;
    public Sprite horizontalSprite;
    public Sprite verticalSprite;
    public Sprite cornerSprite;
    // Tambahkan lagi jika perlu (T-junction, cross, dll)

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Setelah dibuat oleh unit builder
        ConnectWithNearbyWalls();
    }

    public void ConnectWithNearbyWalls()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, connectionRadius, wallLayer);
        List<Vector2> neighborDirs = new();

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Vector2 dir = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
            neighborDirs.Add(dir);

            // Update neighbor agar dia juga cek ulang koneksi
            Wall neighborWall = hit.GetComponent<Wall>();
            neighborWall?.UpdateConnection();
        }

        UpdateConnection(neighborDirs);
    }

    public void UpdateConnection(List<Vector2> dirs = null)
    {
        if (dirs == null)
        {
            // Rehitung sendiri jika tidak dikasih
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, connectionRadius, wallLayer);
            dirs = new();
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject) continue;
                dirs.Add(((Vector2)hit.transform.position - (Vector2)transform.position).normalized);
            }
        }

        // Sederhana: jika kiri-kanan → horizontal, atas-bawah → vertical
        bool left = dirs.Exists(d => Vector2.Dot(d, Vector2.left) > 0.7f);
        bool right = dirs.Exists(d => Vector2.Dot(d, Vector2.right) > 0.7f);
        bool up = dirs.Exists(d => Vector2.Dot(d, Vector2.up) > 0.7f);
        bool down = dirs.Exists(d => Vector2.Dot(d, Vector2.down) > 0.7f);

        if ((left && right) && !up && !down)
            spriteRenderer.sprite = horizontalSprite;
        else if ((up && down) && !left && !right)
            spriteRenderer.sprite = verticalSprite;
        else if ((right && down) || (left && up)) // contoh sudut
            spriteRenderer.sprite = cornerSprite;
        else
            spriteRenderer.sprite = soloSprite;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, connectionRadius);
    }
#endif
}
