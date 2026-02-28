using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class VRFootsteps : MonoBehaviour
{
    public float minMoveSpeed = 0.1f;

    private CharacterController controller;
    private AudioSource audioSource;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(
            controller.velocity.x,
            0,
            controller.velocity.z
        );

        bool isMoving =
            controller.isGrounded &&
            horizontalVelocity.magnitude > minMoveSpeed;

        if (isMoving)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play(); // starts immediately
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
