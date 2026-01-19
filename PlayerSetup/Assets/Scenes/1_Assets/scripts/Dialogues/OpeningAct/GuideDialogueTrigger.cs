using UnityEngine;
using UnityEngine.XR;

public class GuideDialogueTrigger : MonoBehaviour
{
    [SerializeField] private GuideIntro guideIntro;

    private bool playerInside;
    private bool lastBState;
    private bool enabledInteraction;

    public void EnableInteraction()
    {
        enabledInteraction = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabledInteraction) return;
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    void Update()
    {
        if (!enabledInteraction || !playerInside)
            return;

        if (IsBPressed())
        {
            guideIntro.TriggerDialogue();
            enabled = false; // prevent retrigger
        }
    }

    bool IsBPressed()
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
