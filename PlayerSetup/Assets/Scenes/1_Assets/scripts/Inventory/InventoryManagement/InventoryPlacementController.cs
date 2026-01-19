using UnityEngine;

public class InventoryPlacementController : MonoBehaviour
{
    public enum PlacementMode
    {
        FixedInWorld,      // stays where it was opened
        FollowAnchor       // follows player
    }

    [Header("Placement")]
    public PlacementMode placementMode = PlacementMode.FixedInWorld;

    public Transform anchor;
    public float forwardOffset = 0.4f;
    public float verticalOffset = 0.2f;

    private bool active;

    private void OnEnable()
    {
        active = true;
        Place();
    }

    private void OnDisable()
    {
        active = false;
    }

    private void LateUpdate()
    {
        if (!active)
            return;

        if (placementMode == PlacementMode.FollowAnchor)
        {
            Place();
        }
    }

    private void Place()
    {
        transform.position =
            anchor.position +
            anchor.forward * forwardOffset +
            Vector3.up * verticalOffset;

        Vector3 flatForward = anchor.forward;
        flatForward.y = 0f;

        if (flatForward.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(flatForward, Vector3.up);
        }
    }
}
