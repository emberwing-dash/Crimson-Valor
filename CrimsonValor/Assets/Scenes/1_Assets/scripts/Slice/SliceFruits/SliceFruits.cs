using UnityEngine;

public class SliceFruits : MonoBehaviour
{
    [Header("Slice Parts")]
    [SerializeField] private GameObject[] sliceObjects;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionForce = 3f;
    [SerializeField] private float explosionRadius = 1.5f;
    [SerializeField] private float upwardModifier = 0.5f;

    [Header("Sword Settings")]
    [SerializeField] private string swordTag = "Sword";

    private bool sliced = false;
    private FruitSpawner spawner;

    // Called by FruitSpawner
    public void Init(FruitSpawner fruitSpawner)
    {
        spawner = fruitSpawner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (sliced) return;
        if (!other.CompareTag(swordTag)) return;

        sliced = true;
        Slice();
    }

    private void Slice()
    {
        foreach (GameObject obj in sliceObjects)
        {
            if (obj == null) continue;

            obj.transform.SetParent(null);

            Collider col = obj.GetComponent<Collider>();
            if (col != null)
                col.isTrigger = false;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
                rb = obj.AddComponent<Rigidbody>();

            rb.useGravity = true;
            rb.mass = 1f;

            rb.AddExplosionForce(
                explosionForce,
                transform.position,
                explosionRadius,
                upwardModifier,
                ForceMode.Impulse
            );
        }

        // 🔴 IMPORTANT PART (parent cleanup)
        Rigidbody parentRb = GetComponent<Rigidbody>();
        if (parentRb != null)
            Destroy(parentRb);   // stops physics fighting the pieces

        Collider parentCol = GetComponent<Collider>();
        if (parentCol != null)
            parentCol.enabled = false;

        MeshRenderer parentMr = GetComponent<MeshRenderer>();
        if (parentMr != null)
            parentMr.enabled = false;

        if (spawner != null)
            spawner.RegisterSlice();

        Destroy(gameObject, 0.1f);
    }

}
