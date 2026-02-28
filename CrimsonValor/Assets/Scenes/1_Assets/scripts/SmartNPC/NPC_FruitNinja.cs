using UnityEngine;

public class NPC_FruitNinja : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueTyper dialogueTyper;
    [SerializeField] private Animator lenaAnimator;

    [Header("Fruit Ninja Object")]
    [SerializeField] private GameObject fruitNinjaObject;

    [Header("Pointing")]
    [SerializeField] private Transform lenaTransform;
    [SerializeField] private Transform pointTarget;
    [SerializeField] private string pointingBool = "isPointing";

    [Header("Forced Dialogue")]
    [TextArea(2, 4)]
    [SerializeField]
    private string forcedDialogue =
        "Ah! You mean Fruits Ninja! Look over there.";

    private bool triggered;

    /* ---------------- MAIN ENTRY ---------------- */

    // Call this from NPC_Response AFTER NPC text is received
    public void CheckNPCDialogue(string npcText)
    {
        if (triggered) return;
        if (string.IsNullOrEmpty(npcText)) return;

        string lower = npcText.ToLower();

        if (lower.Contains("fruits") && lower.Contains("ninja"))
        {
            triggered = true;
            ActivateFruitNinja();
        }
    }

    /* ---------------- ACTION ---------------- */

    private void ActivateFruitNinja()
    {
        // Activate object
        if (fruitNinjaObject != null)
            fruitNinjaObject.SetActive(true);

        // Point animation
        if (lenaAnimator != null)
            lenaAnimator.SetBool(pointingBool, true);

        // Rotate Lena toward target
        if (lenaTransform != null && pointTarget != null)
        {
            Vector3 dir = pointTarget.position - lenaTransform.position;
            dir.y = 0;
            lenaTransform.rotation = Quaternion.LookRotation(dir);
        }

        // Force dialogue
        if (dialogueTyper != null)
        {
            dialogueTyper.ShowDialogueUI();
            dialogueTyper.StartDialogue(new string[] { forcedDialogue });
        }
    }

    /* ---------------- OPTIONAL RESET ---------------- */

    public void ResetPointing()
    {
        if (lenaAnimator != null)
            lenaAnimator.SetBool(pointingBool, false);
    }
}
