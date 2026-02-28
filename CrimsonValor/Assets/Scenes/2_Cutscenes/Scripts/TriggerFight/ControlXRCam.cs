using System.Collections;
using UnityEngine;

[System.Serializable]
public class TimedGameObject
{
    public GameObject obj;
    public float duration = 3f;
}

public class ControlXRCam : MonoBehaviour
{
    [SerializeField] private TimedGameObject[] objects;

    private int currentIndex = 0;

    void Start()
    {
        // Disable all objects
        foreach (var entry in objects)
        {
            if (entry.obj != null)
                entry.obj.SetActive(false);
        }

        // Enable first object
        if (objects != null && objects.Length > 0)
        {
            currentIndex = 0;
            objects[currentIndex].obj.SetActive(true);
            StartCoroutine(SwitchObjects());
        }
    }

    private IEnumerator SwitchObjects()
    {
        while (currentIndex < objects.Length - 1)
        {
            yield return new WaitForSeconds(objects[currentIndex].duration);

            // Disable current
            if (objects[currentIndex].obj != null)
                objects[currentIndex].obj.SetActive(false);

            // Move to next
            currentIndex++;

            // Enable next
            if (objects[currentIndex].obj != null)
                objects[currentIndex].obj.SetActive(true);
        }

        // Stops after last object
    }
}
