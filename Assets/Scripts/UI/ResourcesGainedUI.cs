using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Resources;
using System.Collections;
using System.Collections.Generic;
public class ResourcesGainedUI : MonoBehaviour
{
    [SerializeField] private GameObject resourceGainedPrefab;
    [SerializeField] private float transformSpeed = 20f; // Speed at which the text moves upwards
    [SerializeField] private float fadeDuration = 2f; // Duration of the fade effect
    [SerializeField] private float delayDuration = 0.1f;

    private Queue<(ResourceType, int)> resourceQueue = new Queue<(ResourceType, int)>();
    private bool isProcessingQueue = false;

    void Start()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogWarning("ResourceManager doesn't exist. Cannot load resources.");
        }
    }

    public void DisplayResourceGained(ResourceType type, int amount)
    {
        resourceQueue.Enqueue((type, amount));
        if (!isProcessingQueue)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isProcessingQueue = true;
        while (resourceQueue.Count > 0)
        {
            var (type, amount) = resourceQueue.Dequeue();
            SpawnPrefab(type, amount);
            yield return new WaitForSeconds(delayDuration);
        }
        isProcessingQueue = false;
    }

    private void SpawnPrefab(ResourceType type, int amount)
    {
        GameObject resourceGainedObj = Instantiate(resourceGainedPrefab, transform);
        resourceGainedObj.GetComponent<TextMeshProUGUI>().text = $"+{amount} {type.ToString()}";
        resourceGainedObj.transform.GetChild(0).GetComponent<Image>().sprite = GetResourceIcon(type);
        StartCoroutine(FadeAndDestroy(resourceGainedObj));
    }

    private Sprite GetResourceIcon(ResourceType type)
    {
        Sprite loadedSprite = Resources.Load<Sprite>($"ResourceIcons/{type.ToString()}");
        if (loadedSprite == null)
        {
            Debug.LogWarning("No icon found for resource type: " + type);
        }
        return loadedSprite;
    }

    private IEnumerator FadeAndDestroy(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            obj.transform.Translate(Vector3.up * Time.deltaTime * transformSpeed); // Move the text upwards over time
            yield return null;
        }

        Destroy(obj);
    }
}
