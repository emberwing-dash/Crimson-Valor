using UnityEngine;

public class NekoSceneTrigger : MonoBehaviour
{
    [SerializeField] GameObject Neko;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Neko.SetActive(true);
        }
    }
}
