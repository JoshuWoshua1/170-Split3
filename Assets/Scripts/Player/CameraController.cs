using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float baseOrthographicSize = 5f;
    [SerializeField] private float cameraScaleMultiplier = 0.5f;
    [SerializeField] private Camera mainCamera;

    private Vector3 positionOffset;

    void Start()
    {
        if (cameraPivot == null)
        {
            Debug.LogError("CameraController requires a cameraPivot Transform assigned.");
        }
        positionOffset = transform.position - cameraPivot.position;
    }

    void Update()
    {
        transform.position = cameraPivot.position + positionOffset;
        CheckSizeChange();
    }

    private void CheckSizeChange()
    {
        float playerSize = PlayerController.Instance.size;
        ScaleCamera(playerSize);
    }
    
    public void ScaleCamera(float playerSize)
    {
        float scaleFactor = 1f + (playerSize - 1f) * cameraScaleMultiplier;
        mainCamera.orthographicSize = baseOrthographicSize * scaleFactor;
    }
}
