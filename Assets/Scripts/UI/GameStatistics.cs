using UnityEngine;

public class GameStatistics : MonoBehaviour
{
    private int mostEaten;
    private int mostResources;
    private int mostCrafted;

    private TMPro.TextMeshProUGUI textComponent;
    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found.");
        }
        textComponent = GetComponent<TMPro.TextMeshProUGUI>();
        UpdateText();
    }

    private void Update()
    {
        // nothing should change while on main menu, leaving update in case we want animation or something else
    }

    private void UpdateText()
    {
        mostEaten = GameManager.Instance.RetrieveMostEaten();
        mostResources = GameManager.Instance.RetrieveMostResourcesCollected();
        mostCrafted = GameManager.Instance.RetrieveMostCrafted();

        textComponent.text = "High Scores:\nMost Things Eaten: " + mostEaten + "\nMost Resources Gathered: " + mostResources + "\nMost Things Crafted: " + mostCrafted;
    }
}
