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
}
