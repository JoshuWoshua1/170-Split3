using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ElementUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Amount;
    [SerializeField] private Image icon;
    //[SerializeField] private Image Icon; //add a way to load an icon

    private int amnt = 0;
    private bool notifying = false;
    //private ResourceType resourceName;

    private void Update()
    {
        //this.amnt = Resource.Instance.GetResourceAmount(resourceName);

        if (!notifying)
        this.Amount.text = amnt.ToString();

    }

    public void setAmount(int num, string type)
    {
        if (type == null)
        {
            Debug.LogWarning("Did not set type for setAmount().");
            return;
        }

        switch (type)
        {
            case "add":
                this.amnt += num;
                break;

            case "set":
                this.amnt = num;
                break;

            default:
                break;
        }
    }

    public int getAmount()
    {
        return amnt;
    }

    public void Load(ResourceType name, int amount, string notifText, bool notify)
    {
        //this.resourceName = name;
        this.Name.text = name.ToString();
        if (notify)
        { 
            notifying = true;
            this.Amount.text = notifText;
        } else
        {
        this.Amount.text = amount.ToString();
        }

        //AI used for the below
        //Loads the icon from Resource/ResourceIcons, name of the icon must be the same as the enum.
        Sprite loadedSprite = Resources.Load<Sprite>($"ResourceIcons/{name}");

        if (loadedSprite != null)
            icon.sprite = loadedSprite;
        else
            Debug.LogWarning($"Could not find icon for {name}!");
    }
}
