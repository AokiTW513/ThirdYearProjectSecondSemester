using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Header("Player Status")]
    [SerializeField] private float speed = 10f;
    // private float jumpForce = 3f;
    private Vector2 moveInput = Vector2.zero;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private string currentPlayerDevice;
    Vector3 lookDirection = Vector3.zero;

    [Space(10)]
    [Header("Skill01")]
    [SerializeField] private bool isSkill01;
    [SerializeField] private bool canSkill01;
    [SerializeField] private float skill01MaxTime;
    [SerializeField] private float skill01Speed;
    [SerializeField] private float skill01CDMaxTime;
    private float skill01CDTimer;
    private float skill01Timer;

    [Space(10)]
    [Header("Skill02")]
    [SerializeField] private bool isSkill02;
    [SerializeField] private bool canSkill02;
    [SerializeField] private float skill02CDMaxTime;
    private float skill02CDTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        currentPlayerDevice = playerInput.currentControlScheme;
        Debug.Log($"Using: {currentPlayerDevice}");
    }

    private void Start()
    {
        skill01Timer = skill01MaxTime;
        isSkill01 = false;
        isSkill02 = false;
        canSkill01 = true;
        canSkill02 = true;

        if(!isLocalPlayer && NetworkClient.active)
        {
            playerInput.enabled = false;
        }
    }

    // 當玩家換手把、或是從鍵盤改用手把時，這個會自動執行
    public void OnControlsChanged(PlayerInput input)
    {
        currentPlayerDevice = input.currentControlScheme;
        Debug.Log($"裝置已切換為: {currentPlayerDevice}");
    }

    private void Update()
    {
        if (isLocalPlayer && NetworkClient.active)
        {
            PlayerMovement();
        }
        else if(!NetworkClient.active)
        {
            PlayerMovement();
        }
    }

    private void PlayerMovement()
    {
        lookDirection = Vector3.zero;
        if (currentPlayerDevice == "Keyboard&Mouse")
        {
            lookDirection = CalculateMouseLook();
        }

        if(!isSkill01 && !isSkill02)
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

        //Skill CD
        if (!canSkill01 && !isSkill01)
        {
            if(skill01CDTimer < 0)
            {
                canSkill01 = true;   
            }
            else
            {
                skill01CDTimer -= Time.deltaTime;   
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
                skill02CDTimer -= Time.deltaTime;   
            }
        }
    }

    private void PlayerMove(Vector2 input, Vector3 lookDirection, string playerDevice)
    {
        if (playerDevice == "Gamepad")
        {
            // 只有當玩家有推搖桿時才旋轉，否則放開搖桿角色會瞬間轉回預設方向
            if (input.sqrMagnitude > 0.02f) 
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

        Vector3 moveDir = new Vector3(input.x, 0f, input.y).normalized;
        rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);

        // Debug.Log($"Player{netId}'s MoveDir:{moveDir}, Using: {playerDevice}, lookDirection: {lookDirection}");
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
    private void Skill01()
    {
        if(skill01Timer > 0)
        {
            rb.linearVelocity = transform.forward * skill01Speed;
            skill01Timer -= Time.deltaTime;
            Debug.Log("OMG is Skill01 :O");
        }
        else
        {
            isSkill01 = false;
            skill01CDTimer = skill01CDMaxTime;
            skill01Timer = skill01MaxTime;
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

    public void OnSkill01(InputAction.CallbackContext callbackContext)
    {
        if (canSkill01 && !isSkill01 && !isSkill02)
        {
            isSkill01 = true;
            canSkill01 = false;
        }   
    }

    public void OnSkill02(InputAction.CallbackContext callbackContext)
    {
        if (canSkill02 && !isSkill01 && !isSkill02)
        {
            isSkill02 = true;
            canSkill02 = false;
        }   
    }
    #endregion
}