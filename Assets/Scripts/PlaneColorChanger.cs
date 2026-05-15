using UnityEngine;

public class PlaneColorChanger : MonoBehaviour
{
    [Header("Plane Reference")]
    [SerializeField] private Renderer planeRenderer;

    [Header("Color Progression (Microscopic to Macro)")]
    [SerializeField] private Color microscopicColor = new Color(0.8f, 0.9f, 1f, 1f);
    [SerializeField] private Color epidermisColor = new Color(1f, 0.85f, 0.85f, 1f);
    [SerializeField] private Color dermisColor = new Color(1f, 0.6f, 0.6f, 1f);
    [SerializeField] private Color skinColor = new Color(1f, 0.8f, 0.7f, 1f);

    [Header("Size Thresholds")]
    [SerializeField] private float size1Threshold = 1f;
    [SerializeField] private float size2Threshold = 3f;
    [SerializeField] private float size3Threshold = 5f;
    [SerializeField] private float size4Threshold = 7f;

    [Header("Transition Settings")]
    [SerializeField] private float transitionSpeed = 1f;

    private Color targetColor;
    private Color currentColor;

    void Start()
    {
        if (planeRenderer == null)
        {
            planeRenderer = GetComponent<Renderer>();
        }

        if (planeRenderer != null)
        {
            currentColor = microscopicColor;
            planeRenderer.material.color = currentColor;
            targetColor = currentColor;
        }
        else
        {
            Debug.LogError("PlaneColorChanger: No Renderer found!");
        }
    }

    void Update()
    {
        if (PlayerController.Instance == null || planeRenderer == null) return;

        float playerSize = PlayerController.Instance.size;
        targetColor = GetColorForSize(playerSize);

        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * transitionSpeed);
        planeRenderer.material.color = currentColor;
    }

    private Color GetColorForSize(float size)
    {
        if (size < size2Threshold)
        {
            float t = Mathf.InverseLerp(size1Threshold, size2Threshold, size);
            return Color.Lerp(microscopicColor, epidermisColor, t);
        }
        else if (size < size3Threshold)
        {
            float t = Mathf.InverseLerp(size2Threshold, size3Threshold, size);
            return Color.Lerp(epidermisColor, dermisColor, t);
        }
        else if (size < size4Threshold)
        {
            float t = Mathf.InverseLerp(size3Threshold, size4Threshold, size);
            return Color.Lerp(dermisColor, skinColor, t);
        }
        else
        {
            return skinColor;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = microscopicColor;
        Gizmos.DrawWireSphere(transform.position, 1f);

        Gizmos.color = epidermisColor;
        Gizmos.DrawWireSphere(transform.position, 2f);

        Gizmos.color = dermisColor;
        Gizmos.DrawWireSphere(transform.position, 3f);

        Gizmos.color = skinColor;
        Gizmos.DrawWireSphere(transform.position, 4f);
    }
}
