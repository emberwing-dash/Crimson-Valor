using UnityEngine;
using UnityEngine.XR;
using System.Collections;

public class BobQuery : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueTyper dialogueTyper;
    [SerializeField] private string[] queryDialogues;
    [SerializeField] private string[] finalDialogues;

    [Header("Name UI")]
    [SerializeField] private GameObject nameTextField;

    [Header("Resource Check")]
    [SerializeField] private GameObject resourceHolder;
    [SerializeField] private string requiredTag;
    [SerializeField] private int requiredCount = 3;

    [Header("Transition Objects")]
    [SerializeField] private GameObject House;
    [SerializeField] private GameObject hideObj;

    [Header("Animation")]
    [SerializeField] private BobAnim bobAnim;

    private bool playerInRange;
    private bool queryEnabled;
    private bool queryStarted;
    private bool finalStarted;
    private bool lastBState;

    /* ---------------- PUBLIC ---------------- */

    public void EnableQueryAfterDelay(float delay)
    {
        StartCoroutine(EnableQueryRoutine(delay));
    }

    private IEnumerator EnableQueryRoutine(float delay)
    {
        queryEnabled = false;
        yield return new WaitForSeconds(delay);
        lastBState = true;
        queryEnabled = true;
    }

    /* ---------------- UPDATE ---------------- */

    private void Update()
    {
        if (!playerInRange || !queryEnabled)
            return;

        // ▶ PLAYER PRESSES B
        if (!queryStarted && !finalStarted && IsBPressed())
        {
            // 🔥 CHECK CONDITION FIRST
            if (CheckResources())
            {
                finalStarted = true;

                bobAnim.SetHappy();

                if (nameTextField != null)
                    nameTextField.SetActive(true);

                dialogueTyper.StartDialogue(finalDialogues);
            }
            else
            {
                queryStarted = true;

                bobAnim.SetAsk();

                if (nameTextField != null)
                    nameTextField.SetActive(true);

                dialogueTyper.StartDialogue(queryDialogues);
            }

            return;
        }

        // ▶ QUERY FINISHED
        if (queryStarted && dialogueTyper.IsDialogueFinished)
        {
            queryStarted = false;

            bobAnim.SetIdle();

            if (nameTextField != null)
                nameTextField.SetActive(false);
        }

        // ▶ FINAL FINISHED
        if (finalStarted && dialogueTyper.IsDialogueFinished)
        {
            finalStarted = false;

            bobAnim.SetIdle();

            if (nameTextField != null)
                nameTextField.SetActive(false);

            DoTransition();
            enabled = false;
        }
    }

    /* ---------------- TRIGGER ---------------- */

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (nameTextField != null)
                nameTextField.SetActive(false);

            bobAnim.SetIdle();
        }
    }

    /* ---------------- LOGIC ---------------- */

    private bool CheckResources()
    {
        int count = 0;
        foreach (Transform child in resourceHolder.transform)
        {
            if (child.CompareTag(requiredTag))
                count++;
        }

        return count >= requiredCount;
    }

    private void DoTransition()
    {
        if (House != null)
            House.SetActive(true);

        if (hideObj != null)
            hideObj.SetActive(false);
    }

    /* ---------------- VR INPUT ---------------- */

    private bool IsBPressed()
    {
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (!rightHand.isValid)
            return false;

        bool pressed;
        rightHand.TryGetFeatureValue(CommonUsages.secondaryButton, out pressed);

        bool result = pressed && !lastBState;
        lastBState = pressed;

        return result;
    }
}
