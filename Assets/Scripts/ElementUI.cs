using UnityEngine;
using TMPro;

public class ElementUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Amount;
    //[SerializeField] private Image Icon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Load(ResourceType name, int amount)
    {
        this.Name.text = name.ToString();
        this.Amount.text = amount.ToString();
    }
}
