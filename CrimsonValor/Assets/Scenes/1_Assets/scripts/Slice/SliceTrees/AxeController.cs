using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class AxeController : MonoBehaviour
{
    [Header("Axe")]
    [SerializeField] private Collider axeTipCollider; // make this a trigger in inspector

    [Header("Hit Sound")]
    [SerializeField] private AudioSource hitAudioSource;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private float hitCooldown = 0.25f;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool canHit = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDestroy()
    {
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    /* ---------------- TREE HIT ---------------- */
    private void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;
        if (!other.CompareTag("Tree")) return;

        TreeController tree = other.GetComponentInParent<TreeController>();
        if (tree == null) return;

        tree.RegisterHit();

        if (hitAudioSource && hitClip)
            hitAudioSource.PlayOneShot(hitClip);

        canHit = false;
        Invoke(nameof(ResetHit), hitCooldown);
    }

    private void ResetHit()
    {
        canHit = true;
    }

    /* ---------------- XR RELEASE ---------------- */
    private void OnReleased(SelectExitEventArgs args)
    {
        // Restore physics on release
        rb.isKinematic = false;
        rb.useGravity = true;

        // Apply interactor velocity if it exists
        if (args.interactorObject is Component interactor)
        {
            Rigidbody interactorRb = interactor.GetComponent<Rigidbody>();
            if (interactorRb != null)
                rb.linearVelocity = interactorRb.linearVelocity;
        }

        rb.angularVelocity = Vector3.zero;
        rb.WakeUp();
    }
}
