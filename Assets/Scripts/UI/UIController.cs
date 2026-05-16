using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private CraftingUI craftingUI;
    [SerializeField] private ProgressUIScript progressUI;
    [SerializeField] private ResourceUIScript resourceUI;
    private bool UIState = false;
    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.tabKey.wasPressedThisFrame) //change this later
        {
            if (!UIState)
            {
                Debug.Log("showing UI");
                ShowUI();
            }
            else
            {
                Debug.Log("hiding UI");
                HideUI();
            }
            
        }
    }

    void ShowUI()
    {
        craftingUI.ToggleCraftingUI();
        progressUI.ToggleProgressPanel();
        resourceUI.ToggleResourcePanel();
    }

    void HideUI()
    {
        craftingUI.ToggleCraftingUI();
        progressUI.ToggleProgressPanel();
        resourceUI.ToggleResourcePanel();
    }
}
