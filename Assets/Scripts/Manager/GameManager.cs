using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mirror.Examples.Basic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{    
    public static GameManager Instance { get; private set;}

    [SerializeField] private int playerCount;
    [SerializeField] private int minPlayer;
    [SerializeField] private int maxGameTime;
    [SerializeField] private float currentGameTime;
    [SerializeField] private int maxCountTime;
    [SerializeField] private int currentCountTime;
    [SerializeField] private int maxShowWinnerTime;
    [SerializeField] private List<GameObject> playerSpawnPoints;
    public GameObject itemSpawnPoint;
    [SerializeField] private GameObject itemPrefab;
    public bool isPause { get; private set;}
    public bool isCounting { get; private set;}
    public bool isPlaying { get; private set;}
    public bool isInLobby { get; private set;}
    private List<GameObject> players = new List<GameObject>();
    private int winPlayerID;
    private Coroutine disableCoroutine;
    private GameObject itemObject;

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

    private void Update()
    {
        if(isPlaying && !isPause)
        {
            if(currentGameTime >= 0)
            {
                currentGameTime -= Time.deltaTime;
                UIManager.Instance.ChangeGameTimer(currentGameTime);
            }
            else
            {
                currentCountTime = 0;
                GameOver(); 
            }
        }        
    }

    public int OnNewPlayer(GameObject obj)
    {
        playerCount++;
        players.Add(obj);
        return playerCount;
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

                itemObject = Instantiate(itemPrefab, itemSpawnPoint.transform);

                //Tp Players to Spawn Points
                foreach(GameObject player in players)
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    playerController.TPToSpawnPoint(playerSpawnPoints[playerController.playerID - 1].transform.position);
                    playerController.Initialized();
                }

                UIManager.Instance.ToggleCountDownTimer(true);
                if(disableCoroutine != null)
                {
                    StopCoroutine(disableCoroutine);
                }
                disableCoroutine = StartCoroutine(StartCountDown());
            }
            else
            {
                UIManager.Instance.ChangeCountDownTimer("老兄你也許需要個朋友...");
                UIManager.Instance.ToggleCountDownTimer(true);
                if(disableCoroutine != null)
                {
                    StopCoroutine(disableCoroutine);
                }
                disableCoroutine = StartCoroutine(TrunOffWinText());
            }
        }
   }

    private IEnumerator StartCountDown()
    {
        if(currentCountTime > 0)
        {
            Debug.Log(currentCountTime);
            UIManager.Instance.ChangeCountDownTimer(currentCountTime.ToString());
            currentCountTime -= 1;
            yield return new WaitForSeconds(1);
            disableCoroutine = StartCoroutine(StartCountDown());
        }
        else
        {
            if(currentCountTime != -1)
            {
                Debug.Log("Fight!!!");
                UIManager.Instance.ChangeCountDownTimer("Fight!!!");
                isCounting = false;
                isPlaying = true;
                UIManager.Instance.ToggleGameTimer(true);
                currentCountTime -= 1;
                yield return new WaitForSeconds(1);
                disableCoroutine = StartCoroutine(StartCountDown());
            }
            else
            {
                //讓他在多一秒才會關掉Fight!!!
                yield return new WaitForSeconds(1);
                UIManager.Instance.ToggleCountDownTimer(false);
            }
        }
    }

    private void GameOver()
    {
        isPlaying = false;
        UIManager.Instance.ToggleGameTimer(false);

        winPlayerID = itemObject.GetComponentInChildren<Item>().GetWinPlayerID();

        //偷拿倒數用的Text來顯示玩家勝利
        if (winPlayerID == 0)
        {
            Debug.Log("Wait. How does nobody win this game lmao");
            UIManager.Instance.ChangeCountDownTimer("Wait. How does nobody win this game lmao");
        }
        else
        {
            Debug.Log($"Player {winPlayerID} Win!!!");
            UIManager.Instance.ChangeCountDownTimer($"Player {winPlayerID} Win!!!");
        }

        UIManager.Instance.ToggleCountDownTimer(true);
        if(disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(TrunOffWinText());

        winPlayerID = 0;
    }

    private IEnumerator TrunOffWinText()
    {
        yield return new WaitForSeconds(maxShowWinnerTime);
        UIManager.Instance.ToggleCountDownTimer(false);
        isInLobby = true;
        Item item = itemObject.GetComponentInChildren<Item>();
        if(item.nowGetItemPlayer != null)
        {
            item.nowGetItemPlayer.GetComponent<PlayerController>().itemObject = null;
        }
        item.ClearGetItemPlayer();
        Destroy(itemObject);
    }
}