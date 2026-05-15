using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // add scriptable object reference for resource thresholds for size upgrades.
    [SerializeField] private SizeUp_Data sizeUpData;

    [SerializeField] private ProgressUIScript progScript; //haphazard solution to issue
    public static PlayerController Instance { get; private set; }
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
        if (characterController == null && gameObject != null)
        {
            characterController = gameObject.GetComponent<CharacterController>();
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

    void Update()
    {
        CheckResourceThresholds(); // check for size upgrade thresholds each frame. may want to optimize this later.
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
        // Add logic to smoothly interpolate the player's scale to the new size.
        StartCoroutine(SmoothScale(Vector3.one * size, 0.5f));
        // Add logic to check for resource thresholds for size upgrades from scriptabe object.
    }

    private IEnumerator SmoothScale(Vector3 targetScale, float duration)
    {
        Vector3 initialScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void CheckResourceThresholds()
    {
        foreach (SizeTier tier in sizeUpData.tiers)
        {
            bool meetsThresholds = true;
            foreach (ResourceRequirement requirement in tier.resourceThresholds)
            {
                if (ResourceManager.Instance.GetResourceAmount(requirement.resourceType) < requirement.amount)
                {
                    meetsThresholds = false;
                    break;
                }
            }
            if (meetsThresholds && size < tier.targetSize)
            {
                progScript.updateProgress();
                foreach (ResourceRequirement requirement in tier.resourceThresholds)
                {
                    ResourceManager.Instance.RemoveResource(requirement.resourceType, requirement.amount); // remove resources used for upgrade from resource manager.
                }
                SetSize(tier.targetSize);
                break; // exit loop after upgrading to the first tier that meets thresholds.
            }
        }
    }
}
