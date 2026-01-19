using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimationStep
{
    public string parameterName;
    public bool useTrigger;
    public float duration;
}

public class AnimationSequenceController : MonoBehaviour
{
    [Header("Core")]
    public Animator animator;
    public AnimationStep[] sequence;

    [Header("Ready")]
    public string readyBool = "isReady";   // 👈 lowercase
    public float readyDuration = 2f;

    private int currentIndex = 0;
    private Coroutine routine;

    void OnEnable()
    {
        if (animator == null || sequence == null || sequence.Length == 0)
            return;

        StartCoroutine(StartAfterReady());
    }

    IEnumerator StartAfterReady()
    {
        // Enable ready state
        animator.SetBool(readyBool, true);

        // Let ready animation play
        yield return new WaitForSeconds(readyDuration);

        // Disable ready
        animator.SetBool(readyBool, false);

        // Now play sequence
        StartSequence();
    }

    IEnumerator PlaySequence()
    {
        while (currentIndex < sequence.Length)
        {
            AnimationStep step = sequence[currentIndex];

            ResetSequenceBools();

            if (step.useTrigger)
                animator.SetTrigger(step.parameterName);
            else
                animator.SetBool(step.parameterName, true);

            yield return new WaitForSeconds(step.duration);

            if (!step.useTrigger)
                animator.SetBool(step.parameterName, false);

            currentIndex++;
        }

        routine = null;
    }

    void ResetSequenceBools()
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool &&
                param.name != readyBool)   // ✅ DO NOT reset isReady
            {
                animator.SetBool(param.name, false);
            }
        }
    }

    // ---------------- PUBLIC ----------------

    public void StartSequence()
    {
        if (routine != null)
            StopCoroutine(routine);

        currentIndex = 0;
        routine = StartCoroutine(PlaySequence());
    }

    public void StopSequence()
    {
        if (routine != null)
            StopCoroutine(routine);

        ResetSequenceBools();
        routine = null;
    }
}
