using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class Item1 : MonoBehaviour
{
    [SerializeField] private GameObject item;
    [SerializeField] private GameObject ItemPrefab;
    private Vector3 center = new Vector3(0, 0, 10);
    private float radius = 10f;

    public void GetItem()
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;

        Vector3 spawnPos = center +
            new Vector3(randomCircle.x, 20, randomCircle.y);

        Instantiate(ItemPrefab, spawnPos, Quaternion.identity);

        Item1Manager.Instance.ChangeCanvaPosition(new Vector3(spawnPos.x, -5.002f, spawnPos.z));
        Item1Manager.Instance.ToggleCanve(true);

        Destroy(item.gameObject);
    }
}