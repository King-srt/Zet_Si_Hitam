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
        if (!Input.GetMouseButtonDown(0)) return;

        BaseUnit selected = BaseUnit.selectedUnit;
        if (selected is Worker worker)
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
