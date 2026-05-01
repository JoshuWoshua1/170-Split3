using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    [SerializeField] private GameObject player;
    [SerializeField] private float moveSpeed = 5f; 
    [SerializeField] private float size = 1f;

    private Vector2 moveInput;

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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = Vector3.one * size;
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.fixedDeltaTime, Space.World);
    }

    public void SetSize(float newSize)
    {
        size = newSize;
        transform.localScale = Vector3.one * size;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}
