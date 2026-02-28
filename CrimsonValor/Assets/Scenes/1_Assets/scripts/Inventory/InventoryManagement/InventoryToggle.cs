using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class InventoryToggle : MonoBehaviour
{
    [SerializeField] private GameObject inventoryVisual;
    [SerializeField] private InventoryStorage storage;

    private InputAction yButtonAction;

    private void OnEnable()
    {
        // Create a button action for Y
        yButtonAction = new InputAction(
            "ToggleInventory",
            InputActionType.Button,
            "<XRController>{LeftHand}/secondaryButton"
        );

        yButtonAction.performed += Toggle;
        yButtonAction.Enable();
    }

    private void OnDisable()
    {
        yButtonAction.performed -= Toggle;
        yButtonAction.Disable();
    }

    private void Toggle(InputAction.CallbackContext ctx)
    {
        bool open = !inventoryVisual.activeSelf;

        if (!open)
            storage.StoreAll();
        else
            storage.RestoreAll();

        inventoryVisual.SetActive(open);
    }
}
