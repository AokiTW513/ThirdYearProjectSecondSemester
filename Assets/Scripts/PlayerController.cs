using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.iOS;

public class PlayerController : MonoBehaviour
{
    private float speed = 10f;
    private float jumpForce = 3f;
    private Vector2 moveInput = Vector2.zero;
    private float gamepadSensitivityX = 500f;
    private float yRotation = 0f;
    private Vector2 lookInput = Vector2.zero;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private string playerDevice;
    private Plane plane;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerDevice = playerInput.currentControlScheme;
        Debug.Log($"Using: {playerDevice}");
    }

    private void Start()
    {
        if(playerDevice == "Keyboard&Mouse")
        {
            plane = new Plane(Vector3.up, Vector3.zero);
        }
    }

    private void Update()
    {   
        Vector3 move = new Vector3(moveInput.x, 0f ,moveInput.y) * speed * Time.deltaTime;
        transform.Translate(move, Space.World);

        if (playerDevice == "Gamepad")
        {
            yRotation += lookInput.x;
            transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        else if(playerDevice == "Keyboard&Mouse")
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if(plane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);

                point.y = transform.position.y;

                transform.LookAt(point);
            }
        }
   }

    // private void FixedUpdate()
    // {

    // }

    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        moveInput = callbackContext.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext callbackContext)
    {
        // rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void OnLook(InputAction.CallbackContext callbackContext)
    {
        Vector2 rawInput = callbackContext.ReadValue<Vector2>();

        if (playerDevice == "Gamepad")
        {
            lookInput = new Vector2(rawInput.x * Time.deltaTime * gamepadSensitivityX, 0);
        }
    }
}