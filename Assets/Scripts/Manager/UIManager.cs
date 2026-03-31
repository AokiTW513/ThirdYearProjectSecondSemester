using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{    
    public static UIManager Instance { get; private set;}

    [SerializeField] private Text countDownTimer;
    [SerializeField] private Text gameTimer;
    [SerializeField] private List<GameObject> playerIcons;

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

        ToggleCountDownTimer(false);
        ToggleGameTimer(false);
    }

    public void ToggleCountDownTimer(bool toggle)
    {
        countDownTimer.gameObject.SetActive(toggle);
    }

    public void ChangeCountDownTimer(string text)
    {
        countDownTimer.text = text;   
    }

    public void ToggleGameTimer(bool toggle)
    {
        gameTimer.gameObject.SetActive(toggle);
    }

    public void ChangeGameTimer(float sec)
    {
        gameTimer.text = Mathf.FloorToInt(sec / 60).ToString("00") + ":" + (sec % 60).ToString("00");   
    }

    public void TogglePlayerIcon(int playerID, bool toggle)
    {
        playerIcons[playerID - 1].SetActive(toggle);
    }
}