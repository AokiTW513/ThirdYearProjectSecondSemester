using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Update()
    {
        transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
    }
}