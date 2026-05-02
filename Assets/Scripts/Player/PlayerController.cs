using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // add scriptable object reference for resource thresholds for size upgrades.
    public static PlayerController Instance { get; private set; }
    [SerializeField] private GameObject player;
    [SerializeField] private float moveSpeed = 5f; 
    public float size = 1f;

    private Vector2 moveInput;
    private CharacterController characterController;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of PlayerController detected. Destroying duplicate.");
            Destroy(gameObject);
        }

        characterController = GetComponent<CharacterController>();
        if (characterController == null && player != null)
        {
            characterController = player.GetComponent<CharacterController>();
        }

        if (characterController == null)
        {
            Debug.LogError("PlayerController requires a CharacterController on this object or the assigned player GameObject.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = Vector3.one * size;
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = Quaternion.Euler(0f, 45f, 0f) * new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        if (characterController != null)
        {
            characterController.Move(moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void SetSize(float newSize) // method to update player size.
    {
        size = newSize;
        transform.localScale = Vector3.one * size;
        // Add logic to check for resource thresholds for size upgrades from scriptabe object.
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}
