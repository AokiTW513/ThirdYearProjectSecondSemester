using Mirror;
using Mirror.Examples.Basic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    private float speed = 10f;
    private float jumpForce = 3f;
    private Vector2 moveInput = Vector2.zero;
    private float gamepadSensitivityX = 500f;
    private float yRotation = 0f;
    private Vector2 lookInput = Vector2.zero;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private string currentPlayerDevice;
    Vector3 lookDirection = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        currentPlayerDevice = playerInput.currentControlScheme;
        Debug.Log($"Using: {currentPlayerDevice}");
    }

    private void Start()
    {
        if(!isLocalPlayer && NetworkClient.active)
        {
            playerInput.enabled = false;   
        }
    }

    private void Update()
    {
        if (isLocalPlayer && NetworkClient.active)
        {
            IDK();
            PlayerMovement(moveInput, lookDirection, currentPlayerDevice); 
        }
        else
        {
            IDK();
            PlayerMovement(moveInput, lookDirection, currentPlayerDevice); 
        }
    }

    private void IDK()
    {
        currentPlayerDevice = playerInput.currentControlScheme;
            
        lookDirection = Vector3.zero;
        if (currentPlayerDevice == "Keyboard&Mouse")
        {
            lookDirection = CalculateMouseLook();
        }
        else if(currentPlayerDevice == "Gamepad")
        {
            lookDirection = new Vector3(lookInput.x , 0 ,0);
        }   
    }

    private void PlayerMovement(Vector2 input, Vector3 lookDirection, string playerDevice)
    {
        Vector3 moveDir = new Vector3(input.x, 0f, input.y).normalized;
        rb.linearVelocity = moveDir * speed;

        Debug.Log($"Player{netId}'s MoveDir:{moveDir}, Using: {playerDevice}, lookDirection: {lookDirection}");

        if (playerDevice == "Gamepad")
        {
            yRotation += lookDirection.x;
            transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        else if (playerDevice == "Keyboard&Mouse" && lookDirection != Vector3.zero)
        {
            lookDirection.y = transform.position.y;
            transform.LookAt(lookDirection);
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

    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        moveInput = callbackContext.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext callbackContext)
    {
        Vector2 rawInput = callbackContext.ReadValue<Vector2>();

        if (currentPlayerDevice == "Gamepad")
        {
            lookInput = new Vector2(rawInput.x * Time.deltaTime * gamepadSensitivityX, 0);
        }
    }
}