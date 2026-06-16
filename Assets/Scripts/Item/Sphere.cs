using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Update()
    {
        transform.position -= new Vector3(0, speed * Time.deltaTime, 0);

        if(transform.position.y <= -5)
        {
            Item1Manager.Instance.ToggleCanve(false);
            Destroy(gameObject);
        }
    }

    public void OnHitPlayer()
    {
        Destroy(gameObject);
    }
}