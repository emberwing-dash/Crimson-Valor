using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class AxeController : MonoBehaviour
{
    [Header("Axe")]
    [SerializeField] private Collider axeTipCollider;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (!canHit)
            return;

        if (collision.contacts[0].thisCollider != axeTipCollider)
            return;

        if (!collision.collider.CompareTag("Tree"))
            return;

        TreeController tree = collision.collider.GetComponentInParent<TreeController>();
        if (tree == null)
            return;

        tree.RegisterHit();

        if (hitAudioSource != null && hitClip != null)
            hitAudioSource.PlayOneShot(hitClip);

        canHit = false;
        Invoke(nameof(ResetHit), hitCooldown);
    }

    private void ResetHit()
    {
        canHit = true;
    }

    /* ---------------- XR RELEASE FIX ---------------- */

    private void OnReleased(SelectExitEventArgs args)
    {
        // Always restore physics
        rb.isKinematic = false;
        rb.useGravity = true;

        // Only apply velocity if the interactor HAS a Rigidbody (hands do, sockets don’t)
        Vector3 releaseVelocity = Vector3.zero;

        if (args.interactorObject is Component interactorComponent)
        {
            Rigidbody interactorRb = interactorComponent.GetComponent<Rigidbody>();
            if (interactorRb != null)
            {
                releaseVelocity = interactorRb.linearVelocity;
            }
        }

        rb.linearVelocity = releaseVelocity;
        rb.angularVelocity = Vector3.zero;
        rb.WakeUp();
    }
}
