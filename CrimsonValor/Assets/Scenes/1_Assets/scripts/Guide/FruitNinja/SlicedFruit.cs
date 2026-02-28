using UnityEngine;

public class SlicedFruit : MonoBehaviour
{
    private FruitSpawner spawner;
    private bool sliced = false;

    public void Init(FruitSpawner fruitSpawner)
    {
        spawner = fruitSpawner;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (sliced) return;

        if (collision.gameObject.CompareTag("Sword"))
        {
            sliced = true;
            spawner.RegisterSlice();
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            sliced = true; // prevent later scoring
        }
    }
}
