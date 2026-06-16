using System.Collections;
using UnityEngine;

public class Item1Manager : MonoBehaviour
{
    public static Item1Manager Instance { get; private set;}

    [SerializeField] private GameObject Canva;
    private Vector3 center = new Vector3(0, 0, 10);
    private float radius = 10f;
    [SerializeField] private GameObject Item1Prefab;
    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;
    private Coroutine disableCoroutine;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);   
        }
        else
        {
            Instance = this;   
        }
    }

    public void ToggleCanve(bool toggle)
    {
        Canva.SetActive(toggle);
    }

    public void ChangeCanvaPosition(Vector3 position)
    {
        Canva.transform.position = position;  
    }

    private void RespawnItem1()
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;

        Vector3 spawnPos = center +
            new Vector3(randomCircle.x, -3.9f, randomCircle.y);

        Instantiate(Item1Prefab, spawnPos, Quaternion.identity);
    }

    public void StartSpawnItem1()
    {
        float rnd = Random.Range(minSpawnTime, maxSpawnTime);

        disableCoroutine = StartCoroutine(IDK(rnd));
    }

    private IEnumerator IDK(float rnd)
    {
        yield return new WaitForSeconds(rnd);

        RespawnItem1();

        // StartSpawnItem1();
    }

    public void StopSpawnItem1()
    {
        StopCoroutine(disableCoroutine);
    }
}