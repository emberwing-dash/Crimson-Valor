using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InventoryStorage : MonoBehaviour
{
    [SerializeField] private InventorySocket[] sockets;

    private XRBaseInteractable[] storedItems;

    private void Awake()
    {
        storedItems = new XRBaseInteractable[sockets.Length];
    }

    public void StoreAll()
    {
        for (int i = 0; i < sockets.Length; i++)
        {
            XRBaseInteractable item = sockets[i].GetCurrentItem();
            if (item == null)
                continue;

            storedItems[i] = item;

            XRSocketInteractor socket =
                sockets[i].GetComponent<XRSocketInteractor>();

            socket.interactionManager.SelectExit(
                (IXRSelectInteractor)socket,
                (IXRSelectInteractable)item
            );

            GameObject go = item.gameObject;
            go.transform.SetParent(transform, false);

            if (go.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            go.SetActive(false);
        }
    }

    public void RestoreAll()
    {
        for (int i = 0; i < storedItems.Length; i++)
        {
            XRBaseInteractable item = storedItems[i];
            if (item == null)
                continue;

            GameObject go = item.gameObject;
            go.SetActive(true);

            XRSocketInteractor socket =
                sockets[i].GetComponent<XRSocketInteractor>();

            socket.interactionManager.SelectEnter(
                (IXRSelectInteractor)socket,
                (IXRSelectInteractable)item
            );
        }
    }
}
