using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class WeaponScale : MonoBehaviour
{
    [Header("Socket Tag to Ignore (Optional)")]
    [SerializeField] private string placingSocketTag = "PlacingSocket";

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grabInteractable.trackScale = false; // keep original scale
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

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!(args.interactorObject is XRSocketInteractor socket))
        {
            // Grabbed by hand → ensure physics is normal
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        else
        {
            // Placed into socket → only freeze if you want to lock it
            if (!socket.gameObject.CompareTag(placingSocketTag))
            {
                rb.isKinematic = true;  // lock in socket
                rb.useGravity = false;
            }
            else
            {
                // Ignore scaling/physics, leave normal
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        // Leaving socket → restore physics
        if (args.interactorObject is XRSocketInteractor)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}
