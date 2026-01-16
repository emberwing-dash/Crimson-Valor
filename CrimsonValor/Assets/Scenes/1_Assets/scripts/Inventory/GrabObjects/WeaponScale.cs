using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class WeaponScale : MonoBehaviour
{
    [Header("Scale Settings")]
    [SerializeField] private Vector3 socketScale = new Vector3(0.7f, 0.7f, 0.7f);

    private Vector3 originalScale;
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Prevent XR from overriding scale
        grabInteractable.trackScale = false;

        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    /* ---------------- CORE LOGIC ---------------- */

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // 🧠 If selected by a SOCKET → shrink
        if (args.interactorObject is XRSocketInteractor)
        {
            transform.localScale = socketScale;
        }
        // 🧠 If selected by HAND → keep original size
        else
        {
            transform.localScale = originalScale;
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        // Always restore when leaving socket or hand
        transform.localScale = originalScale;
    }
}
