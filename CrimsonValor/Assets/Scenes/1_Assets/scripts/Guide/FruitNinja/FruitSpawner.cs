using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FruitSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Fruits")]
    [SerializeField] private List<GameObject> fruitList;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private int maxFruits = 20;

    [Header("Score UI")]
    [SerializeField] private Canvas scoreCanvas;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private float scoreShowDelay = 2f;

    [Header("Exit UI")]
    [SerializeField] private Canvas exitCanvas;

    [Header("Name UI")]
    [SerializeField] private TMP_Text nameText;   

    private int slicedCount = 0;

    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn points not assigned!");
            return;
        }

        if (fruitList == null || fruitList.Count == 0)
        {
            Debug.LogError("Fruit list is empty!");
            return;
        }

        if (scoreCanvas != null) scoreCanvas.gameObject.SetActive(false);
        if (exitCanvas != null) exitCanvas.gameObject.SetActive(false);

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < maxFruits; i++)
        {
            SpawnFruit();
            yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitForSeconds(scoreShowDelay);
        ShowScore();
    }

    private void SpawnFruit()
    {
        GameObject fruitPrefab = fruitList[Random.Range(0, fruitList.Count)];
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject fruit = Instantiate(fruitPrefab, point.position, point.rotation);

        SliceFruits slice = fruit.GetComponent<SliceFruits>();
        if (slice != null)
            slice.Init(this);
    }

    public void RegisterSlice()
    {
        slicedCount++;
    }

    private void ShowScore()
    {
        if (scoreCanvas != null) scoreCanvas.gameObject.SetActive(true);
        if (exitCanvas != null) exitCanvas.gameObject.SetActive(true);

        if (scoreText != null)
            scoreText.text = $"Fruits Cut: {slicedCount} / {maxFruits}";

        // Next level
        if (nameText != null)
            nameText.text = "";
    }
}
