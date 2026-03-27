using Mirror;
using UnityEngine;

public class PlayerHitbox : NetworkBehaviour
{
    private PlayerController playerController;
    
    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Skill01")
        {
            if (NetworkClient.active)
            {
                playerController.CmdPush();
            }
            else
            {
                playerController.Push(collider.gameObject);
            }
            Debug.Log("IDK");
        }
    }
}