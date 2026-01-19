using UnityEngine;
using System.Collections;

public class PlankSpawn : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float delaySeconds = 3f;

    [Header("Objects")]
    [SerializeField] private GameObject treeBase;
    [SerializeField] private GameObject plankObject;
    [SerializeField] private GameObject[] originTree; // now an array

    private void OnEnable()
    {
        // Disable all original trees immediately
        if (originTree != null && originTree.Length > 0)
        {
            foreach (GameObject tree in originTree)
            {
                if (tree != null)
                    tree.SetActive(false);
            }
        }

        // fallback if treeBase not set
        if (treeBase == null && originTree != null && originTree.Length > 0)
            treeBase = originTree[0];

        StartCoroutine(HandlePlankSpawn());
    }

    private IEnumerator HandlePlankSpawn()
    {
        yield return new WaitForSeconds(delaySeconds);

        if (treeBase != null)
            treeBase.SetActive(false);

        if (plankObject != null)
            plankObject.SetActive(true);
    }
}
