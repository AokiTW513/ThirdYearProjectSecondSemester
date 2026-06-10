using Mirror;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private Transform itemTransform;
    
    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Skill01")
        {
            playerController.Push(other.gameObject);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.hitSFX, transform.position); 
            Debug.Log("IDK");
        }

        if(other.gameObject.tag == "Item")
        {
            if(other.gameObject.GetComponent<Item>() != null)
            {
                Item item = other.gameObject.GetComponent<Item>();
                item.SetItemParent(itemTransform);
                item.SetGetItemPlayer(this.gameObject);
                playerController.itemObject = item.GetItemParent();
            }
            else if(other.gameObject.GetComponent<Item1>() != null)
            {
                other.gameObject.GetComponent<Item1>().GetItem();
            } 
        }

        if(other.gameObject.tag == "Item2")
        {
            Item1 item1 = other.gameObject.GetComponent<Item1>();   
        }
    }
}