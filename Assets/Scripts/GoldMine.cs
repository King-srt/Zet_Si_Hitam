using UnityEngine;

public class GoldMine : MonoBehaviour
{
    [Header("Gold Settings")]
    public int goldAmount = 100;

    /// <summary>
    /// Ambil emas dari tambang.
    /// </summary>
    public bool MineGold(int amount)
    {
        if (goldAmount <= 0) return false;

        goldAmount -= amount;
        goldAmount = Mathf.Max(0, goldAmount);
        Debug.Log($"⛏️ Mined {amount} gold. Remaining: {goldAmount}");

        return true;
    }

    public bool IsDepleted()
    {
        return goldAmount <= 0;
    }
}

