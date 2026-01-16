using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryToggle : MonoBehaviour
{
    [SerializeField] private GameObject inventoryVisual;
    [SerializeField] private InventoryStorage storage;
    [SerializeField] private InputActionReference toggleAction;

    private void OnEnable()
    {
        toggleAction.action.Enable();
        toggleAction.action.performed += Toggle;
    }

    private void OnDisable()
    {
        toggleAction.action.performed -= Toggle;
        toggleAction.action.Disable();
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
