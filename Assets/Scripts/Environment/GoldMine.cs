using System.Collections;
using UnityEngine;

public class GoldMine : MonoBehaviour
{
    [Header("Gold Settings")]
    public int goldAmount = 100;

    private bool isBeingDestroyed = false;

    public bool IsDepleted()
    {
        return goldAmount <= 0;
    }

    public bool MineGold(int amount)
    {
        if (goldAmount <= 0 || isBeingDestroyed) return false;

        goldAmount -= amount;
        goldAmount = Mathf.Max(0, goldAmount);
        Debug.Log($"⛏️ Mined {amount} gold. Remaining: {goldAmount}");

        if (IsDepleted())
        {
            isBeingDestroyed = true;
            StartCoroutine(FadeAndDestroy(1.0f));
        }

        return true;
    }

    private IEnumerator FadeAndDestroy(float duration)
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (spriteRenderers.Length == 0)
        {
            Destroy(gameObject);
            yield break;
        }

        float elapsed = 0f;
        Color[] originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                Color c = originalColors[i];
                c.a = alpha;
                spriteRenderers[i].color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (BaseUnit.selectedUnit is Worker worker)
            {
                if (!IsDepleted())
                {
                    worker.SetMiningTarget(this);
                    Debug.Log("🟡 Worker dikirim menambang oleh klik kanan.");
                }
                else
                {
                    Debug.Log("⚠️ Tambang ini sudah habis.");
                }
            }
            else
            {
                Debug.Log("❌ Unit terpilih bukan Worker.");
            }
        }
    }
}
