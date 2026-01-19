using UnityEngine;

public class TreeController : MonoBehaviour
{
    [SerializeField] private int hitsToBreak = 3;
    [SerializeField] private GameObject shatteredTree;

    private int currentHits;
    private bool isBroken;

    public void RegisterHit()
    {
        if (isBroken)
            return;

        currentHits++;

        if (currentHits >= hitsToBreak)
            BreakTree();
    }

    private void BreakTree()
    {
        isBroken = true;

        // 🔥 Enable shattered tree immediately
        if (shatteredTree != null)
            shatteredTree.SetActive(true);

    }
}
