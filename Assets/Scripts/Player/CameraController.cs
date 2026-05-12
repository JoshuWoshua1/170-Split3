using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float baseOrthographicSize = 5f;
    [SerializeField] private float cameraScaleMultiplier = 1f;
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
        float targetSize = baseOrthographicSize + (playerSize - 1f) * cameraScaleMultiplier;
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetSize, Time.deltaTime);
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
}
