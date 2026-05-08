using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{    
    public static GameManager Instance { get; private set;}

    private bool hasAuthority;
    [SerializeField] private int playerCount; //IDK
    [SerializeField] private int minPlayer;
    [SerializeField] private int maxGameTime;
    [SerializeField] private float currentGameTime;
    [SerializeField] private int maxCountTime;
    [SerializeField] private int currentCountTime;
    [SerializeField] private int maxShowWinnerTime;
    [SerializeField] private List<GameObject> playerSpawnPoints;
    public GameObject itemSpawnPoint;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private bool isPause;
    [SerializeField] private bool isCounting;
    [SerializeField] private bool isPlaying;
    [SerializeField] private bool isInLobby;
    private List<GameObject> players = new List<GameObject>();
    public List<int> winPlayerID = new List<int>();
    private Coroutine disableCoroutine;
    private GameObject itemObject;
    public List<float> playerGetItemTime = new List<float>();

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

        isPause = false;
        isPlaying = false;
        isInLobby = true;
    }

    private void Start()
    {
        if (NetworkClient.active || NetworkServer.active)
        {
            hasAuthority = isServer;
        }
        else
        {
            hasAuthority = true;   
        }
    }

    private void Update()
    {
        if(isPlaying && !isPause)
        {
            if(currentGameTime >= 0)
            {
                currentGameTime -= Time.deltaTime;
                LevelUIManager.Instance.ChangeGameTimer(currentGameTime);
            }
            else
            {
                currentCountTime = 0;
                GameOver(); 
            }
        }        
    }

    #region Get Var
    public bool GetIsPause()
    {
        return isPause;
    }

    public bool GetIsCounting()
    {
        return isCounting;
    }

    public bool GetIsInLobby()
    {
        return isInLobby;
    }

    public bool GetIsPlaying()
    {
        return isPlaying;
    }

    public bool GetHasAuthority()
    {
        return hasAuthority;
    }
    #endregion

    #region Toggle Var
    public void TogglePause()
    {
        if (hasAuthority)
        {
            ApplyPause();  
        }
        else
        {
            CmdTogglePause();   
        }
    }

    [Command]
    private void CmdTogglePause()
    {
        ApplyPause();
    }


    private void ApplyPause()
    {
        isPause = !isPause;
        RpcApplyPause();
    }

    [ClientRpc]
    private void RpcApplyPause()
    {
        if (isPause)
        {
            Time.timeScale = 0f;
            LevelUIManager.Instance.TogglePauseText(false);
        }
        else
        {
            Time.timeScale = 1f;   
            LevelUIManager.Instance.TogglePauseText(true);
        }    
    }
    #endregion

    public void OnNewPlayer(GameObject obj)
    {
        if (hasAuthority)
        {
            playerCount++;
            players.Add(obj);
            obj.GetComponent<PlayerController>().SetPlayerID(playerCount);
        }
    }

    public void StartGame()
    {
        if(!isCounting && !isPlaying)
        {
            if(playerCount >= minPlayer)
            {
                currentGameTime = maxGameTime;
                isCounting = true;
                isInLobby = false;
                currentCountTime = maxCountTime;

                itemObject = Instantiate(itemPrefab, itemSpawnPoint.transform.position, itemSpawnPoint.transform.rotation);
                NetworkServer.Spawn(itemObject);

                playerGetItemTime.Clear();
                for(int i = 0; i < playerCount; i++)
                {
                    playerGetItemTime.Add(0);
                }

                //Tp Players to Spawn Points
                foreach(GameObject player in players)
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    playerController.RpcTPToSpawnPoint(playerSpawnPoints[playerController.GetPlayerID() - 1].transform.position);
                    playerController.RpcInitialized();
                }

                RpcStartGame(); 
            }
            else
            {
                LevelUIManager.Instance.ChangeCountDownTimer("老兄你也許需要個朋友...");
                LevelUIManager.Instance.ToggleCountDownTimer(true);
                if(disableCoroutine != null)
                {
                    StopCoroutine(disableCoroutine);
                }
                disableCoroutine = StartCoroutine(TrunOffWinText());
            }
        }
   }

   [ClientRpc]
    private void RpcStartGame()
    {
        LevelUIManager.Instance.ToggleCountDownTimer(true);
        if(disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(StartCountDown());
    }

    private IEnumerator StartCountDown()
    {
        if(currentCountTime > 0)
        {
            Debug.Log(currentCountTime);
            LevelUIManager.Instance.ChangeCountDownTimer(currentCountTime.ToString());
            currentCountTime -= 1;
            yield return new WaitForSeconds(1);
            disableCoroutine = StartCoroutine(StartCountDown());
        }
        else
        {
            if(currentCountTime != -1)
            {
                Debug.Log("Fight!!!");
                LevelUIManager.Instance.ChangeCountDownTimer("Fight!!!");
                isCounting = false;
                isPlaying = true;
                LevelUIManager.Instance.ToggleGameTimer(true);
                currentCountTime -= 1;
                yield return new WaitForSeconds(1);
                disableCoroutine = StartCoroutine(StartCountDown());
            }
            else
            {
                //讓他在多一秒才會關掉Fight!!!
                yield return new WaitForSeconds(1);
                LevelUIManager.Instance.ToggleCountDownTimer(false);
            }
        }
    }

    private void GameOver()
    {
        isPlaying = false;
        LevelUIManager.Instance.ToggleGameTimer(false);

        // winPlayerID = itemObject.GetComponentInChildren<Item>().GetWinPlayerID();

        foreach(GameObject player in players)
        {
            int nowPlayerID = player.GetComponent<PlayerController>().GetPlayerID();
            if(winPlayerID.Count == 0)
            {
                winPlayerID.Add(nowPlayerID);   
            }
            else
            {
                if(playerGetItemTime[nowPlayerID - 1] >= playerGetItemTime[winPlayerID[0] - 1])
                {
                    if(playerGetItemTime[nowPlayerID - 1] == playerGetItemTime[winPlayerID[0] - 1])
                    {
                        winPlayerID.Add(nowPlayerID);   
                    }
                    else
                    {
                        winPlayerID.Clear();
                        winPlayerID.Add(nowPlayerID);   
                    }
                }
            }
        }

        Debug.Log($"Player {string.Join(", ", winPlayerID)} Win!!!");
        LevelUIManager.Instance.ChangeCountDownTimer($"Player {string.Join(", ", winPlayerID)} Win!!!");

        LevelUIManager.Instance.ToggleCountDownTimer(true);
        if(disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(TrunOffWinText());

        winPlayerID.Clear();
    }

    private IEnumerator TrunOffWinText()
    {
        yield return new WaitForSeconds(maxShowWinnerTime);
        LevelUIManager.Instance.ToggleCountDownTimer(false);
        isInLobby = true;
        if(itemObject != null)
        {
            Item item = itemObject.GetComponentInChildren<Item>();
            if(item.nowGetItemPlayer != null)
            {
                item.nowGetItemPlayer.GetComponent<PlayerController>().itemObject = null;
            }
            item.ClearGetItemPlayer();
            Destroy(itemObject);

            playerGetItemTime = new List<float>();
        }
    }

    public void SetPlayerGetItemTime(int playerID, float time)
    {
        if(hasAuthority)
        {
            CmdSetPlayerGetItemTime(playerID, time);   
        }
        else
        {
            ApplySetPlayerGetItemTime(playerID, time);
        }
    }

    [Command]
    private void CmdSetPlayerGetItemTime(int playerID, float time)
    {
        ApplySetPlayerGetItemTime(playerID, time);   
    }

    private void ApplySetPlayerGetItemTime(int playerID, float time)
    {
        playerGetItemTime[playerID - 1] = time;
    }
}