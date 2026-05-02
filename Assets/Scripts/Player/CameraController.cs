using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;

    private Vector3 positionOffset; 
    private Quaternion rotationOffset;

    void Start()
    {
        positionOffset = transform.position - cameraPivot.position;
        rotationOffset = transform.rotation;
    }

    void Update()
    {
        transform.position = cameraPivot.position + positionOffset;
        transform.rotation = rotationOffset;
    }

    public void ScaleCamera(float playersize)
    {
        float scaleFactor = 1f + (playersize - 1f) * 0.5f; // Adjust the multiplier as needed
        positionOffset *= scaleFactor;
    }
}
