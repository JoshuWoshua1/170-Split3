using UnityEngine;

// Game manager, keep very simple
// only put things here that must be reset on game end, and must
// be accessable by multiple scripts, or between scenes.
// Most varialbes here should be reset upon game end, since this script
// is persistant between scenes, and will not be reset upon reloading the scene.

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Reset these on game end ----
    [SerializeField] private int thingsEaten = 0; // total things eaten this run, (not shown during gameplay)
    [SerializeField] private int mostThingsEaten = 0; // psuedo high score, shown in main menu

    [SerializeField] private int resourcesCollected = 0; // total resources collected this run, (not shown during gameplay)
    [SerializeField] private int mostResourcesCollected = 0; // psuedo high score

    [SerializeField] private int thingsCrafted = 0; // total things crafted this run, (not shown during gameplay)
    [SerializeField] private int mostThingsCrafted = 0; // psuedo high score

    // ----------------------------

    public void AddEaten(int points)
    {
        thingsEaten += points;
        if (thingsEaten > mostThingsEaten)
        {
            mostThingsEaten = thingsEaten;
        }
    }

    public int RetrieveMostEaten()
    {
        //Debug.Log("Current Highest consumed: " + mostThingsEaten);
        return mostThingsEaten;
    }

    public void AddResourcesGained(int points)
    {
        resourcesCollected += points;
        if (resourcesCollected > mostResourcesCollected)
        {
            mostResourcesCollected = resourcesCollected;
        }
    }

    public int RetrieveMostResourcesCollected()
    {
        //Debug.Log("Current Highest resources collected: " + mostResourcesCollected);
        return mostResourcesCollected;
    }

    public void AddCraftedCount(int points)
    {
        thingsCrafted += points;
        if (thingsCrafted > mostThingsCrafted)
        {
            mostThingsCrafted = thingsCrafted;
        }
    }

    public int RetrieveMostCrafted()
    {
        //Debug.Log("Current Highest crafted: " + mostThingsCrafted);
        return mostThingsCrafted;
    }

    public void ResetGame()
    {
        thingsEaten = 0;
        resourcesCollected = 0;
        thingsCrafted = 0;
    }
}
