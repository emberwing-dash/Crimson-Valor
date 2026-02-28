using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC_EmotionAnimator : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Animator Bool Names (must match Animator EXACTLY)")]
    [SerializeField] private string talkingBool = "isTalking";
    [SerializeField] private string annoyedBool = "isAnnoyed";
    [SerializeField] private string headPainBool = "isHeadPain";
    [SerializeField] private string angryBool = "isAngry";

    private string lastEmotion = "Idle";

    private readonly List<string> emotions = new()
    {
        "Talking",
        "Annoyed",
        "HeadPain",
        "Angry"
    };

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        ResetToIdle();
    }

   //set emotion in the form of switch-case

    public void SetEmotion(string emotion)
    {
        if (string.IsNullOrEmpty(emotion))
            emotion = "Talking";

        // Prevent same animation twice
        if (emotion == lastEmotion)
            emotion = PickDifferentEmotion(emotion);

        lastEmotion = emotion;

        ResetToIdle();

        switch (emotion)
        {
            case "Talking":
                animator.SetBool(talkingBool, true);
                break;

            case "Annoyed":
                animator.SetBool(annoyedBool, true);
                break;

            case "Upset":
                animator.SetBool(headPainBool, true);
                break;

            case "Angry":
                animator.SetBool(angryBool, true);
                break;
        }
    }

    

    public void ResetToIdle()
    {
        animator.SetBool(talkingBool, false);
        animator.SetBool(annoyedBool, false);
        animator.SetBool(headPainBool, false);
        animator.SetBool(angryBool, false);

        lastEmotion = "Idle";
    }

   

    private string PickDifferentEmotion(string current)
    {
        List<string> options = new(emotions);
        options.Remove(current);

        return options[Random.Range(0, options.Count)];
    }
}
