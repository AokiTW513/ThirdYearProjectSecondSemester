using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float speed = 10f;
    private float jumpForce = 3f;
    private Vector2 moveInput = Vector2.zero;
    private float mouseSensitivityX = 0.1f;
    private float gamepadSensitivityX = 500f;
    private float xRotation = 0f;
    private Vector2 lookInput = Vector2.zero;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {   
        Vector3 move = new Vector3(moveInput.x, 0f ,moveInput.y) * speed * Time.deltaTime;
        transform.Translate(move, Space.Self);

        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
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
        var device = callbackContext.control.device;
        Vector2 rawInput = callbackContext.ReadValue<Vector2>();

        if (device is Mouse)
        {
            lookInput = new Vector2(rawInput.x * mouseSensitivityX, 0);
        }
        else if (device is Gamepad)
        {
            lookInput = new Vector2(rawInput.x * Time.deltaTime * gamepadSensitivityX, 0);
        }
    }
}