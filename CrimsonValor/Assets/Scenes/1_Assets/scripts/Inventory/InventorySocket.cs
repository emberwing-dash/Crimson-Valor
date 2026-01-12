using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class InventorySocket : MonoBehaviour
{
    private XRSocketInteractor socket;

    private void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnEnter);
        socket.selectExited.AddListener(OnExit);
    }

    private void OnEnter(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        args.interactableObject.transform.localPosition = Vector3.zero;
        args.interactableObject.transform.localRotation = Quaternion.identity;
    }

    private void OnExit(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    public XRBaseInteractable GetCurrentItem()
    {
        return socket.hasSelection
            ? socket.interactablesSelected[0] as XRBaseInteractable
            : null;
    }
}
