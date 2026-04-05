using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class Item : NetworkBehaviour
{
    [SyncVar] public GameObject nowGetItemPlayer;
    private Rigidbody rb;
    private BoxCollider boxCollider;
    public bool canGet { get; private set;}
    [SyncVar] private float cannotGetTimer;

    [SerializeField] private float respawnY;
    [SerializeField] private float maxCannotGetTime;
    [SerializeField] private GameObject itemParent;
    [SerializeField] private float force;
    [SerializeField] private float forceY;
    [SerializeField] private float syncTime;
    private float syncTimer;
    private Vector3 targetPosition;
    [SerializeField] private float positionLerpSpeed;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        canGet = true;
        syncTimer = syncTime;
    }

    private void Update()
    {
        if (!canGet && GameManager.Instance.GetHasAuthority())
        {
            if(cannotGetTimer >= 0)
            {
                cannotGetTimer -= Time.deltaTime;   
            }
            else
            {
                canGet = true;
                boxCollider.enabled = true;
            }
        }

        //OutRange
        if(itemParent.transform.position.y < -10 && GameManager.Instance.GetHasAuthority())
        {
            int x = Random.Range(-8, 8);
            int z = Random.Range(-8, 8);
            itemParent.transform.position = new Vector3(x, respawnY, z);  
        }

        if (GameManager.Instance.GetHasAuthority() && GameManager.Instance.GetIsPlaying())
        {
            if(syncTimer >= 0)
            {
                syncTimer -= Time.deltaTime;
            }
            else
            {
                syncTimer = syncTime;
                RpcSyncItemPosition(itemParent.transform.position);
            }
        }

        itemParent.transform.position = Vector3.Lerp(itemParent.transform.position, targetPosition, Time.deltaTime * positionLerpSpeed);
    }

    [ClientRpc]
    private void RpcSyncItemPosition(Vector3 pos)
    {
        targetPosition = pos;
    }

    public void SetGetItemPlayer(GameObject player)
    {
        nowGetItemPlayer = player.GetComponentInParent<PlayerController>().gameObject;
        rb.isKinematic = true;
        boxCollider.enabled = false;
        Debug.Log($"Player {nowGetItemPlayer.GetComponent<PlayerController>().GetPlayerID()} Get Item!");
    }

    public void ClearGetItemPlayer()
    {
        itemParent.transform.SetParent(GameManager.Instance.itemSpawnPoint.transform, true);
        nowGetItemPlayer = null;
    }

    public int GetWinPlayerID()
    {
        if(nowGetItemPlayer != null)
        {
            return nowGetItemPlayer.GetComponent<PlayerController>().GetPlayerID();  
        }
        else
        {
            return 0;
        }
    }

    public void DropItem(GameObject obj)
    {
        Debug.Log($"Player {nowGetItemPlayer.GetComponent<PlayerController>().GetPlayerID()} Drop Item!");
        rb.isKinematic = false;
        Vector3 horizontalForce = -obj.transform.forward * force;
        Vector3 verticalForce = Vector3.up * forceY;
        rb.AddForce(horizontalForce + verticalForce, ForceMode.Impulse);
        ClearGetItemPlayer();
        canGet = false;
        cannotGetTimer = maxCannotGetTime; //讓他一段時間不能被拿，不然會掉了後本人或敵人秒拿._.
    }

    public void DropItem()
    {
        Debug.Log($"Player {nowGetItemPlayer.GetComponent<PlayerController>().GetPlayerID()} Drop Item!");
        rb.isKinematic = false;
        ClearGetItemPlayer();
        canGet = false;
        cannotGetTimer = maxCannotGetTime; //讓他一段時間不能被拿，不然會掉了後本人或敵人秒拿._.
    }
    
    public void SetItemParent(Transform transform)
    {
        itemParent.transform.SetParent(transform, false);
        itemParent.transform.localPosition = new Vector3(0, 0, 0);
        itemParent.transform.localRotation = Quaternion.identity;
    }

    public GameObject GetItemParent()
    {
        return itemParent;   
    }
}