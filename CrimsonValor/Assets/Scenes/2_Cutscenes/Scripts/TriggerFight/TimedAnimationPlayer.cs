using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
public class TimedAnimation
{
    public string stateName;   // Exact Animator State name
    public float duration;     // How long to play it
}

public class TimedAnimationPlayer : MonoBehaviour
{
    [Header("Core")]
    public Animator animator;

    [Header("Start Delay")]
    public float startDelay = 2f;

    [Header("Animation List")]
    public TimedAnimation[] animations;

    private Coroutine routine;

    void OnEnable()
    {
        if (animator == null || animations == null || animations.Length == 0)
            return;

        routine = StartCoroutine(PlayAnimations());
    }

    IEnumerator PlayAnimations()
    {
        // ⏱ Wait before starting
        yield return new WaitForSeconds(startDelay);

        foreach (var anim in animations)
        {
            if (string.IsNullOrEmpty(anim.stateName))
                continue;

            animator.Play(anim.stateName);

            yield return new WaitForSeconds(anim.duration);
        }

        routine = null;
    }

    // Optional manual control
    public void Restart()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(PlayAnimations());
    }










    public void nextSceneLoad()
    {
        SceneManager.LoadScene(2);
    }
}
