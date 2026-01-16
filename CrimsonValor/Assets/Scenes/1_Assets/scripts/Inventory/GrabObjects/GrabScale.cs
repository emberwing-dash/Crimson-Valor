using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

[RequireComponent(typeof(XRGrabInteractable))]
public class GrabScale : MonoBehaviour
{
    [Header("Scale Settings")]
    [SerializeField] private Vector3 grabbedScale = new Vector3(0.7f, 0.7f, 0.7f);

    private Vector3 originalScale;
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.trackScale = false; // REQUIRED
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // 🟢 wait one frame so grab fully registers
        StartCoroutine(ScaleNextFrame(grabbedScale));
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        transform.localScale = originalScale;
    }

    private IEnumerator ScaleNextFrame(Vector3 targetScale)
    {
        yield return null; // wait 1 frame
        transform.localScale = targetScale;
    }
}
