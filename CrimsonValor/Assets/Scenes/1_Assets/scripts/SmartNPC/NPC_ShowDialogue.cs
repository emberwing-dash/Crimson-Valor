using UnityEngine;
using UnityEngine.XR;
using TMPro;

public class NPC_ShowDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject dialogueRoot;
    [SerializeField] private TMP_Text nameText;

    private bool playerInside;
    private bool lastBState;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            ForceHide(); // optional but clean
        }
    }

    private void Update()
    {
        if (!playerInside) return;

        if (IsBPressed())
        {
            ToggleDialogue();
        }
    }

    /* ---------------- TOGGLE ---------------- */

    private void ToggleDialogue()
    {
        if (dialogueRoot == null) return;

        bool newState = !dialogueRoot.activeSelf;

        dialogueRoot.SetActive(newState);

        if (nameText != null)
            nameText.gameObject.SetActive(newState);
    }

    private void ForceHide()
    {
        if (dialogueRoot != null)
            dialogueRoot.SetActive(false);

        if (nameText != null)
            nameText.gameObject.SetActive(false);
    }

    /* ---------------- VR INPUT ---------------- */

    private bool IsBPressed()
    {
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (!rightHand.isValid) return false;

        bool pressed;
        rightHand.TryGetFeatureValue(CommonUsages.secondaryButton, out pressed);

        bool result = pressed && !lastBState;
        lastBState = pressed;

        return result;
    }
}
