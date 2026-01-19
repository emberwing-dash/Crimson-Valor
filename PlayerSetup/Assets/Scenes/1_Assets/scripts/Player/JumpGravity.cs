using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

public class VRJumpGravityFix : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float extraFallMultiplier = 2f;

    private CharacterController controller;
    private float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        ApplyGravity();
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            // Small stick-to-ground force
            verticalVelocity = -2f;
        }
        else
        {
            // Force gravity even when holding objects
            verticalVelocity += gravity * extraFallMultiplier * Time.deltaTime;
        }

        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }
}
