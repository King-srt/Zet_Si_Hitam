using System;
using System.Collections;
using UnityEngine;

public class GoldMine : MonoBehaviour
{
    [Header("Gold Settings")]
    public int goldAmount = 100;

    public delegate void OnGoldMined(int amount);
    public static event OnGoldMined GoldMined;

    public bool IsDepleted()
    {
        return goldAmount <= 0;
    }

    private bool isBeingDestroyed = false;

    public bool MineGold(int amount)
    {
        if (goldAmount <= 0 || isBeingDestroyed) return false;

        goldAmount -= amount;
        goldAmount = Mathf.Max(0, goldAmount);
        GoldMined?.Invoke(amount);
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
}

