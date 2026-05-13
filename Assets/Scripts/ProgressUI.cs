using TMPro;
using UnityEngine;

public class ProgressUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Current;
    [SerializeField] private TextMeshProUGUI Required;

    private int amnt = 0;
    private int required = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.Current.text = amnt.ToString();
        this.Required.text = required.ToString();
    }

    public void setAmount(int cur, int req, string type)
    {
        if (type == null)
        {
            Debug.LogWarning("Did not set type for setAmount().");
            return;
        }

        switch (type)
        {
            case "add":
                this.amnt += cur;
                this.required += req;
                break;

            case "set":
                this.amnt = cur;
                this.required = req;
                break;

            default:
                break;
        }
    }

    public void Load(ResourceType name, int curr, int req)
    {
        this.Name.text = name.ToString();
        this.Current.text = curr.ToString();
        this.Required.text = req.ToString();
    }

}
