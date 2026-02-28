using UnityEngine;
using UnityEngine.XR;

public class VRVibrator : MonoBehaviour
{
    // Vibrate left controller
    public void VibrateLeft(float amplitude, float duration)
    {
        Vibrate(XRNode.LeftHand, amplitude, duration);
    }

    // Vibrate right controller
    public void VibrateRight(float amplitude, float duration)
    {
        Vibrate(XRNode.RightHand, amplitude, duration);
    }

    // Vibrate both controllers
    public void VibrateBoth(float amplitude, float duration)
    {
        VibrateLeft(amplitude, duration);
        VibrateRight(amplitude, duration);
    }

    // Core vibration function
    private void Vibrate(XRNode hand, float amplitude, float duration)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(hand);
        if (!device.isValid) return;

        if (device.TryGetHapticCapabilities(out HapticCapabilities caps) && caps.supportsImpulse)
        {
            device.SendHapticImpulse(0, Mathf.Clamp01(amplitude), duration);
        }
    }
}
