using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{    
    public static LevelUIManager Instance { get; private set;}

    [SerializeField] private Text countDownTimer;
    // [SerializeField] private Text gameTimer;
    [SerializeField] private List<GameObject> playerIcons;
    [SerializeField] private GameObject pauseText;
    [SerializeField] private List<Sprite> timerSprites;
    [SerializeField] private GameObject gameTimerUI;
    [SerializeField] private List<Image> timerImages;

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
        ToggleGameTimerUI(false);
    }

    public void ToggleCountDownTimer(bool toggle)
    {
        countDownTimer.gameObject.SetActive(toggle);
    }

    public void ChangeCountDownTimer(string text)
    {
        countDownTimer.text = text;   
    }

    public void ToggleGameTimerUI(bool toggle)
    {
        gameTimerUI.gameObject.SetActive(toggle);
    }

    public void ChangeGameTimer(int sec)
    {
        string timerString = sec.ToString("D3");

        timerImages[0].sprite = timerSprites[int.Parse(timerString[0].ToString())];
        timerImages[1].sprite = timerSprites[int.Parse(timerString[1].ToString())];
        timerImages[2].sprite = timerSprites[int.Parse(timerString[2].ToString())];
    }

    public void TogglePlayerIcon(int playerID, bool toggle)
    {
        playerIcons[playerID - 1].SetActive(toggle);
    }

    public void TogglePauseText(bool toggle)
    {
        pauseText.SetActive(toggle);   
    }
}