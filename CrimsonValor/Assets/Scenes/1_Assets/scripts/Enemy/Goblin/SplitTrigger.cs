using UnityEngine;

public class SplitTrigger : MonoBehaviour
{
    [Header("Split")]
    [SerializeField] private GameObject splitObj;
    [SerializeField] private float force = 3f;

    [Header("Disable After Hit")]
    [SerializeField] private BoxCollider[] splitTriggers;
    [SerializeField] private SkinnedMeshRenderer[] goblinMeshes; // All goblin meshes

    [Header("Goblin Club")]
    [SerializeField] private GameObject clubObj;

    [Header("Goblin References")]
    [SerializeField] private GameObject goblinRoot;
    [SerializeField] private GoblinManager goblinManager;

    private BoxCollider myCol;

    private void Awake()
    {
        myCol = GetComponent<BoxCollider>();
        myCol.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Sword")) return;

        // Disable split triggers
        foreach (BoxCollider col in splitTriggers)
            if (col) col.enabled = false;

        // Hide all goblin meshes
        foreach (var mesh in goblinMeshes)
            if (mesh) mesh.enabled = false;

        // Hide club
        if (clubObj) clubObj.SetActive(false);

        // Unparent and enable split object
        if (splitObj)
        {
            splitObj.transform.SetParent(null);
            splitObj.SetActive(true);

            Rigidbody[] rbs = splitObj.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                Vector3 dir = (splitObj.transform.position - rb.position).normalized;
                rb.AddForce(dir * force, ForceMode.Impulse);
            }
        }

        // Disable Goblin AI only
        var ai = goblinRoot.GetComponent<GoblinAI>();
        if (ai) ai.enabled = false;

        // Notify manager
        if (goblinManager && goblinRoot)
            goblinManager.GoblinDied(goblinRoot);
    }
}
