using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject mainMeunUI;
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject createGameUI;
    [SerializeField] private GameObject joinUI;

    [Space(10)]
    [Header("MainMenuUI")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button quitButton;

    [Space(10)]
    [Header("StartUI")]
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button startUIBackButton;

    [Space(10)]
    [Header("CreateGameUI")]
    [SerializeField] private Button hostLocalGameButton;
    [SerializeField] private Button hostOnlineGameButton;
    [SerializeField] private Button createGameUIBackButton;

    [Space(10)]
    [Header("JoinUI")]
    [SerializeField] private InputField ipInputField;
    [SerializeField] private Button JoinGameButton;
    [SerializeField] private Button joinUIBackButton;
    
    private void Awake()
    {
        //MainMenuUI
        startButton.onClick.AddListener(() =>
        {
            ToggleMainMenuUI(false);
            ToggleStartUI(true);
        });
        settingButton.onClick.AddListener(() => Debug.Log("Clicked Setting Button."));
        quitButton.onClick.AddListener(Application.Quit);
        
        //StartUI
        createGameButton.onClick.AddListener(() =>
        {
            ToggleStartUI(false);
            ToggleCreateGameUI(true);
        });
        joinButton.onClick.AddListener(() =>
        {
            ToggleMainMenuUI(true);
            ToggleJoinUI(false);
        });
        startUIBackButton.onClick.AddListener(() =>
        {
            ToggleMainMenuUI(true);
            ToggleStartUI(false);
        });

        //CreateGameUI
        hostLocalGameButton.onClick.AddListener(() => Debug.Log("Clicked Host Local Game Button."));
        hostOnlineGameButton.onClick.AddListener(() => Debug.Log("Clicked Host Online Game Button."));
        createGameUIBackButton.onClick.AddListener(() =>
        {
            ToggleStartUI(true);
            ToggleCreateGameUI(false);
        });

        //JoinUI
        JoinGameButton.onClick.AddListener(() =>
        {
            if(ipInputField.text != "")
            {
                NetworkManagerScript.Instance.networkAddress = ipInputField.text;
                NetworkManagerScript.Instance.StartClient();
            }
        });
        joinUIBackButton.onClick.AddListener(() =>
        {
            ToggleMainMenuUI(true);
            ToggleJoinUI(false);
        });
    }

    private void ToggleMainMenuUI(bool toggle)
    {
        mainMeunUI.SetActive(toggle);
    }

    private void ToggleStartUI(bool toggle)
    {
        startUI.SetActive(toggle);
    }

    private void ToggleCreateGameUI(bool toggle)
    {
        createGameUI.SetActive(toggle);
    }

    private void ToggleJoinUI(bool toggle)
    {
        joinUI.SetActive(toggle);
    }
}