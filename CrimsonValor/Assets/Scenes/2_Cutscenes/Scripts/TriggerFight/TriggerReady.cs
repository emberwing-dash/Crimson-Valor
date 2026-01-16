using UnityEngine;

public class TriggerReady : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {

        }
    }
}
