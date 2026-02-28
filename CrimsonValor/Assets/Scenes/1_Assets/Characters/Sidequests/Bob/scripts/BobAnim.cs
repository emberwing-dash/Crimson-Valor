using UnityEngine;

public class BobAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int IsAsk = Animator.StringToHash("isAsk");
    private static readonly int IsHappy = Animator.StringToHash("isHappy");

    /* ---------------- STATES ---------------- */

    public void SetIdle()
    {
        animator.SetBool(IsAsk, false);
        animator.SetBool(IsHappy, false);
    }

    public void SetAsk()
    {
        animator.SetBool(IsAsk, true);
        animator.SetBool(IsHappy, false);
    }

    public void SetHappy()
    {
        animator.SetBool(IsAsk, false);
        animator.SetBool(IsHappy, true);
    }
}
