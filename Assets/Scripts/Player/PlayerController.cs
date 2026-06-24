using kcp2k;
using Mirror;
using Mirror.Examples.Basic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Var
    [Header("Player Status")]
    [SerializeField] private float gamePadDeadZone;
    [SerializeField] private float speed;
    // private float jumpForce = 3f;
    private Vector2 moveInput = Vector2.zero;
    private Rigidbody rb;
    private Animator animator;
    private PlayerInput playerInput;
    private string currentPlayerDevice;
    Vector3 lookDirection = Vector3.zero;
    private bool isPushed;
    public bool isDizziness;
    public float dizzinessMaxTime;
    public float dizzinessTimer = 0;
    private int playerID;
    public GameObject itemObject;
    private float getItemTime;
    [SerializeField] private Transform characterPrefabTransform;
    [SerializeField] private float currentShameMeter;
    [SerializeField] private float maxShameMeter; 
    [SerializeField] private float maxShameMeterSegments; 

    [Space(10)]
    [Header("ATK")]
    [SerializeField] private bool isATKCharge;
    [SerializeField] private bool isATK;
    [SerializeField] private bool canATK;
    [SerializeField] private BoxCollider ATKHitbox;
    [SerializeField] private float ATKMaxTime;
    [SerializeField] private float ATKSpeed;
    [SerializeField] private float ATKCDMaxTime;
    [SerializeField] private float ATKForce;
    [SerializeField] private float ATKForceY;
    private float ATKCDTimer;
    [SerializeField] private float ATKTimer;

    [Space(10)]
    [Header("Skill01")]
    [SerializeField] private bool isSkill01;
    [SerializeField] private bool canSkill01;
    [SerializeField] private BoxCollider skill01Hitbox;
    [SerializeField] private float skill01MaxTime;
    [SerializeField] private float skill01Speed;
    [SerializeField] private float skill01CDMaxTime;
    [SerializeField] private float skill01Force;
    [SerializeField] private float skill01ForceY;
    private float skill01CDTimer;
    private float skill01Timer;

    [Space(10)]
    [Header("Skill02")]
    [SerializeField] private bool isSkill02;
    [SerializeField] private bool canSkill02;
    [SerializeField] private float skill02CDMaxTime;
    private float skill02CDTimer;

    [Space(10)]
    [Header("Parry")]
    [SerializeField] private bool isParry;
    [SerializeField] private bool canParry;
    [SerializeField] private float parryCDMaxTime;
    private float parryCDTimer;
    private float parryTimer;
    [SerializeField] private float parryMaxTimer;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        currentPlayerDevice = playerInput.currentControlScheme;
        Debug.Log($"Using: {currentPlayerDevice}");
    }

    private void Start()
    {
        Initialized();

        GameManager.Instance.OnNewPlayer(this.gameObject);
        LevelUIManager.Instance.TogglePlayerIcon(playerID, true);
        gameObject.name = $"Player {playerID}";

        GameObject character = Instantiate(GameManager.Instance.characterPrefabs[playerID - 1], characterPrefabTransform, false);
        character.transform.localPosition = new Vector3(0, -1, 0);
        animator = character.GetComponentInChildren<Animator>();
    }

    public void Initialized()
    {
        isPushed = false;
        skill01Timer = skill01MaxTime;
        isSkill01 = false;
        isSkill02 = false;
        canSkill01 = true;
        canSkill02 = true;

        ATKTimer = 0;
        isATKCharge = false;
        canATK = true;

        isParry = false;
        canParry = true;
        parryTimer = parryMaxTimer;

        rb.linearVelocity = new Vector3(0, 0, 0);
        getItemTime = 0;
    }

    public void SetPlayerID(int id)
    {
        playerID = id;
    }

    public int GetPlayerID()
    {
        return playerID;   
    }

    // 當玩家換手把、或是從鍵盤改用手把時，這個會自動執行
    public void OnControlsChanged(PlayerInput input)
    {
        currentPlayerDevice = input.currentControlScheme;
        Debug.Log($"裝置已切換為: {currentPlayerDevice}");
    }

    private void FixedUpdate()
    {
        if(!GameManager.Instance.GetIsInLobby() && !GameManager.Instance.GetIsPlaying() && !GameManager.Instance.GetIsPause()) return;

        PlayerMovement();
        // Debug.Log($"Player{netId} Using: {currentPlayerDevice}");
    }

    private void Update()
    {
        GetItemTime();

        NotItemTime();
    }

    private void PlayerMovement()
    {
        lookDirection = Vector3.zero;
        if (currentPlayerDevice == "Keyboard&Mouse")
        {
            lookDirection = CalculateMouseLook();
        }

        if(!isSkill01 && !isSkill02 && !isATK && !isParry && !isPushed && !isDizziness)
        {
            PlayerMove(moveInput, lookDirection, currentPlayerDevice);
        }
        else if(isSkill01)
        {
            Skill01();
        }
        else if(isSkill02)
        {
            Skill02(); 
        }
        else if(isATK)
        {
            ATK(); 
        }
        else if(isParry)
        {
            if(parryTimer > 0)
            {
                parryTimer -= Time.deltaTime;
            }
            else
            {
                isParry = false;
                parryTimer = parryMaxTimer;
            }
        }

        if (isDizziness)
        {
            if(dizzinessTimer > 0)
            {
                dizzinessTimer -= Time.deltaTime;
            }
            else
            {
                dizzinessTimer = 0;
                isDizziness = false;
            }   
        }

        //ATK
        if (!canATK && !isATK && !isATKCharge)
        {
            if(ATKCDTimer < 0)
            {
                canATK = true;
            }
            else
            {
                ATKCDTimer -= Time.fixedDeltaTime;   
            }
        }
        ATKCharge();

        //Parry
        if (!isParry && !canParry)
        {
            if(parryCDTimer < 0)
            {
                canParry = true; 
            }
            else
            {
                parryCDTimer -= Time.fixedDeltaTime;
            }
        }

        //Skill CD
        if (!canSkill01 && !isSkill01)
        {
            if(skill01CDTimer < 0)
            {
                canSkill01 = true;   
            }
            else
            {
                skill01CDTimer -= Time.fixedDeltaTime;   
            }
        }
        if (!canSkill02 && !isSkill02)
        {
            if(skill02CDTimer < 0)
            {
                canSkill02 = true;   
            }
            else
            {
                skill02CDTimer -= Time.fixedDeltaTime;   
            }
        }

        //OutRange
        if(transform.position.y < -10)
        {
            if(itemObject != null)
            {
                itemObject.GetComponentInChildren<Item>().DropItem();
                itemObject = null;
            }

            int x = Random.Range(-8, 8);
            int z = Random.Range(-8, 8);
            transform.position = new Vector3(x, 1, z);
            isPushed = false;
            animator.SetBool("IsHit", false);
        }
    }

    private void GetItemTime()
    {
        if (itemObject != null)
        {
            getItemTime += Time.deltaTime;
            GameManager.Instance.SetPlayerGetItemTime(playerID, getItemTime); 
        }
    }

    private void NotItemTime()
    {
        if (itemObject == null && currentShameMeter < maxShameMeter)
        {
            currentShameMeter += Time.deltaTime;
        }
        else
        {
            currentShameMeter = maxShameMeter;
        }
    }

    private void PlayerMove(Vector2 input, Vector3 lookDirection, string playerDevice)
    {
        if (playerDevice == "Gamepad")
        {
            // 只有當玩家有推搖桿時才旋轉
            if (input.sqrMagnitude > gamePadDeadZone) 
            {
                Vector3 targetDirection = new Vector3(input.x, 0f, input.y);
                // 使用 Quaternion.LookRotation 算出該方向的旋轉值
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                
                // 如果想要「瞬間」轉向：
                transform.rotation = targetRotation;

                // 如果想要「平滑」轉向（手感更好）：
                // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
            }
        }
        else if (playerDevice == "Keyboard&Mouse" && lookDirection != Vector3.zero)
        {
            lookDirection.y = transform.position.y;
            transform.LookAt(lookDirection);
        }

        if (!isPushed)
        {
            Vector3 moveDir = new Vector3(input.x, 0f, input.y).normalized;
            rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);
            // Debug.Log( rb.linearVelocity.y);
            // Debug.Log($"Player{netId}'s MoveDir:{moveDir}, Using: {playerDevice}, lookDirection: {lookDirection}");

            if(moveDir != Vector3.zero)
            {
                animator.SetBool("IsRunning", true);
            }
            else
            {
                animator.SetBool("IsRunning", false);
            }
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }

    private Vector3 CalculateMouseLook()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(Vector3.up, transform.position);
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }

    #region Skill
    private void ToggleHitBox(int skillNumber, bool toggle)
    {
        StartToggleHitBox(skillNumber, toggle);
    }

    private void StartToggleHitBox(int skillNumber, bool toggle)
    {
        switch (skillNumber)
        {
            case 1:
                skill01Hitbox.enabled = toggle;
                break;
        }  
    }

    private void Skill01()
    {
        if(skill01Timer > 0)
        {
            rb.linearVelocity = transform.forward * skill01Speed;
            skill01Timer -= Time.fixedDeltaTime;
        }
        else
        {
            isSkill01 = false;
            skill01CDTimer = skill01CDMaxTime;
            skill01Timer = skill01MaxTime;
            ToggleHitBox(1, false);
        }
    }

    private void ATKCharge()
    {
        if(ATKTimer <= ATKMaxTime && isATKCharge)
        {
            ATKTimer += Time.fixedDeltaTime;
        }
        else if (isATKCharge)
        {
            ATKTimer = ATKMaxTime;
        }
    }

    private void ATK()
    {
        if(ATKTimer > 0)
        {
            rb.linearVelocity = transform.forward * ATKSpeed;
            ATKTimer -= Time.fixedDeltaTime;
        }
        else
        {
            isATK = false;
            ATKCDTimer = ATKCDMaxTime;
            ATKTimer = 0;
            ToggleHitBox(1, false);
            animator.SetBool("IsATK", false);
        }
    }

    private void Skill02()
    {
        Debug.Log("OMG is Skill02 :O");
        isSkill02 = false;
        skill02CDTimer = skill02CDMaxTime;
    }
    #endregion

    #region Input Control
    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        moveInput = callbackContext.ReadValue<Vector2>();
    }

    public void OnATK(InputAction.CallbackContext callbackContext)
    {
        if(!GameManager.Instance.GetIsInLobby() && !GameManager.Instance.GetIsPlaying() && !GameManager.Instance.GetIsPause()) return;

        if (callbackContext.started && canATK && !isATKCharge)
        {
            isATKCharge = true;
            Debug.Log("OMG is ATK :O");
        }

        if(callbackContext.canceled && isATKCharge)
        {
            isATKCharge = false;
            isATK = true;
            canATK = false;
            ToggleHitBox(1, true);
        }
    }

    public void OnSkill01(InputAction.CallbackContext callbackContext)
    {
        // if(!GameManager.Instance.GetIsInLobby() && !GameManager.Instance.GetIsPlaying() && !GameManager.Instance.GetIsPause()) return;

        // if (canSkill01 && !isSkill01 && !isSkill02)
        // {
        //     isSkill01 = true;
        //     canSkill01 = false;
        //     ToggleHitBox(1, true);
        //     Debug.Log("OMG is Skill01 :O");
        // }

        if(!GameManager.Instance.GetIsInLobby() && !GameManager.Instance.GetIsPlaying() && !GameManager.Instance.GetIsPause()) return;

        if (callbackContext.started && canATK && !isATKCharge)
        {
            isATKCharge = true;
            AudioManager.instance.PlayLoop(FMODEvents.instance.chargeSFX); 
            Debug.Log("OMG is ChargeATK :O");
        }

        if(callbackContext.canceled && isATKCharge)
        {
            AudioManager.instance.StopLoop(); 
            isATKCharge = false;
            isATK = true;
            canATK = false;
            ToggleHitBox(1, true);
            animator.SetBool("IsATK", true);
            Debug.Log("OMG is ATK :O");
            AudioManager.instance.PlayOneShot(FMODEvents.instance.dashSFX, transform.position); 
        }
    }

    public void OnSkill02(InputAction.CallbackContext callbackContext)
    {
        // if(!GameManager.Instance.GetIsInLobby() && !GameManager.Instance.GetIsPlaying() && !GameManager.Instance.GetIsPause()) return;

        // if (canSkill02 && !isSkill01 && !isSkill02)
        // {
        //     isSkill02 = true;
        //     canSkill02 = false;
        // }

        if(!GameManager.Instance.GetIsInLobby() && !GameManager.Instance.GetIsPlaying() && !GameManager.Instance.GetIsPause()) return;

        //Parry Delete
        // if(!isParry && canParry)
        // {
        //     isParry = true;
        //     canParry = false;
        //     Debug.Log("wow is parry");  
        // }
    }

    public void OnParry(InputAction.CallbackContext callbackContext)
    {
        if(!GameManager.Instance.GetIsInLobby() && !GameManager.Instance.GetIsPlaying() && !GameManager.Instance.GetIsPause()) return;

        if(!isParry && canParry)
        {
            isParry = true;
            canParry = false;   
        }
    }
    #endregion

    public void Push(GameObject obj)
    {
        StartPush(obj);
    }

    public void StartPush(GameObject obj)
    {
        if (isParry)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.parrySFX, transform.position); 
            return;
        }

        if (isSkill01)
        {
            isSkill01 = false;
            skill01CDTimer = skill01CDMaxTime;
            skill01Timer = skill01MaxTime;
            ToggleHitBox(1, false);    
        }

        isPushed = true;
        animator.SetBool("IsHit", true);

        isDizziness = false;

        Vector3 horizontalForce = obj.transform.forward * skill01Force;
        Vector3 verticalForce = Vector3.up * skill01ForceY;

        rb.AddForce(horizontalForce + verticalForce, ForceMode.Impulse);

        if(itemObject != null)
        {
            itemObject.GetComponentInChildren<Item>().DropItem(obj);
            itemObject = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            isPushed = false;
            animator.SetBool("IsHit", false);
        }
    }

    public void OnStartButton(InputAction.CallbackContext callbackContext)
    {
        if(playerID == 1)
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            Debug.Log("Bro, you are not Player 1, you cannot start the game.");   
        }
    }

    public void OnQuitButton(InputAction.CallbackContext callbackContext)
    {
        Application.Quit();
    }

    public void TPToSpawnPoint(Vector3 position)
    {
        transform.position = position;   
    }
}