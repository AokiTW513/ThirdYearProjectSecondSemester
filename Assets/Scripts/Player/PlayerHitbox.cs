using Mirror;
using UnityEngine;

public class PlayerHitbox : NetworkBehaviour
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
            Debug.Log("IDK");
        }

        if(other.gameObject.tag == "Item")
        {
            Item item = other.gameObject.GetComponent<Item>();
            item.SetItemParent(itemTransform);
            item.SetGetItemPlayer(this.gameObject);
            playerController.itemObject = item.GetItemParent();
        }
    }
}