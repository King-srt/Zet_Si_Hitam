using UnityEngine;

[RequireComponent(typeof(GoldMine))]
public class GoldMineClickHandler : MonoBehaviour
{
    private GoldMine goldMine;

    private void Awake()
    {
        goldMine = GetComponent<GoldMine>();
    }

    private void OnMouseDown()
    {
        if (!Input.GetMouseButtonDown(0)) return; // klik kiri

        // Pastikan unit yang sedang dipilih adalah worker
        BaseUnit selected = BaseUnit.selectedUnit;
        if (selected is WorkerUnit worker)
        {
            worker.SetMiningTarget(goldMine);
            Debug.Log("📌 Worker dikirim ke tambang!");
        }
        else
        {
            Debug.LogWarning("❌ Unit terpilih bukan Worker.");
        }
    }
}
