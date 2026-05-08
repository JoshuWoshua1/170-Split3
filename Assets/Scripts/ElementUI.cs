using UnityEngine;
using TMPro;

public class ElementUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Amount;
    //[SerializeField] private Image Icon; //add a way to load an icon

    private int amnt = 0;
    //private ResourceType resourceName;

    private void Update()
    {
        //this.amnt = Resource.Instance.GetResourceAmount(resourceName);

        this.Amount.text = amnt.ToString();

        /*if (this.amnt > 0)
        {
            this.SetActive(true);
        } else
        {
            this.SetActive(false);
        }*/
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

    public void Load(ResourceType name, int amount)
    {
        //this.resourceName = name;
        this.Name.text = name.ToString();
        this.Amount.text = amount.ToString();
    }
}
