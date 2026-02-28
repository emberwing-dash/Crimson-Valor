using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoblinManager : MonoBehaviour
{
    [Header("Goblins In Scene (Parent objects)")]
    [SerializeField] private List<GameObject> goblinParents = new List<GameObject>();

    [Header("Quest Cleared UI")]
    [SerializeField] private GameObject questClearedUI;
    [SerializeField] private Animator questAnimator;

    [Header("Reward")]
    [SerializeField] private GameObject paladinReward;

    private int currentIndex = 0; // Tracks which goblin to spawn

    private void Start()
    {
        if (questClearedUI != null) questClearedUI.SetActive(false);
        if (paladinReward != null) paladinReward.SetActive(false);

        // **Leave all children as-is**; don't force enable
        // We'll only enable the goblin we want
        SpawnCurrentGoblin();
    }

    /// <summary>
    /// Enable the next goblin child under the current parent.
    /// </summary>
    private void SpawnCurrentGoblin()
    {
        if (currentIndex >= goblinParents.Count)
        {
            QuestCleared();
            return;
        }

        GameObject parent = goblinParents[currentIndex];
        if (!parent)
        {
            currentIndex++;
            SpawnCurrentGoblin();
            return;
        }

        // Only enable the child goblin we want
        if (parent.transform.childCount > 0)
        {
            Transform goblinChild = parent.transform.GetChild(0); // first child only
            goblinChild.gameObject.SetActive(true);

            // Ensure renderers are visible
            foreach (var renderer in goblinChild.GetComponentsInChildren<Renderer>())
                renderer.enabled = true;

            // Reset AI
            GoblinAI ai = goblinChild.GetComponent<GoblinAI>();
            if (ai != null)
            {
                ai.InitializePatrol();
                ai.enabled = true;
            }

            // Reset NavMeshAgent
            NavMeshAgent agent = goblinChild.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                agent.isStopped = false;
            }
        }
    }

    /// <summary>
    /// Called by SplitTrigger when a goblin is "killed".
    /// </summary>
    public void GoblinDied(GameObject goblinChild)
    {
        if (goblinChild)
        {
            // Disable AI only, leave parent active
            GoblinAI ai = goblinChild.GetComponent<GoblinAI>();
            if (ai != null) ai.enabled = false;

            // Hide goblin mesh
            foreach (var renderer in goblinChild.GetComponentsInChildren<Renderer>())
                renderer.enabled = false;

            // Optionally, hide club if needed
            Transform club = goblinChild.transform.Find("Club");
            if (club) club.gameObject.SetActive(false);
        }

        currentIndex++;
        SpawnCurrentGoblin();
    }

    private void QuestCleared()
    {
        if (questClearedUI != null) questClearedUI.SetActive(true);
        if (questAnimator != null) StartCoroutine(WaitForQuestAnimation());
        else EnableReward();
    }

    private IEnumerator WaitForQuestAnimation()
    {
        yield return null; // wait one frame
        AnimatorStateInfo state = questAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length);
        EnableReward();
    }

    private void EnableReward()
    {
        if (paladinReward != null) paladinReward.SetActive(true);
    }
}
