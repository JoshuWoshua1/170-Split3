using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine.InputSystem;
using System;

public class ResourceUIScript : MonoBehaviour
{

    [SerializeField] private ElementUI UIPrefab;
    [SerializeField] private Transform UITransform;
    [SerializeField] private Transform UITransformParent;
    [SerializeField] private TextMeshProUGUI tabText;

    private Dictionary<ResourceType, ElementUI> UIElements = new Dictionary<ResourceType, ElementUI>();
    private Queue<Action> notifQueue = new Queue<Action>();

    private float xPos = 0;

    private bool animPlaying = false;
    private bool showingPanel = false;
    private bool notifying = false;

    void Start()
    {
        //DOTween.Init(autoKillMode, useSafeMode, logBehaviour); 

        if (ResourceManager.Instance == null)
        {
            Debug.LogWarning("ResourceManager doesn't exist. Cannot load resources.");
        }

        InitializeUI();
    }

    void Update()
    {
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            checkResource(type, ResourceManager.Instance.GetResourceAmount(type));
        }

        if (UnityEngine.InputSystem.Keyboard.current.qKey.wasPressedThisFrame) //change this later
        {
            Debug.Log("showing panel");
            if (!animPlaying)
                showPanel();
        }
        if (UnityEngine.InputSystem.Keyboard.current.qKey.wasReleasedThisFrame) //change this later
        {
            Debug.Log("hiding panel");
            if (!animPlaying)
                hidePanel();
        }

      /*  if (!notifying && !showingPanel)
            showNextNotify();*/
    }

    void InitializeUI()
    {
        UITransformParent.Translate(Vector3.left * 350);

        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            ElementUI newElement = Instantiate(UIPrefab, UITransform);
            newElement.Load(type, ResourceManager.Instance.GetResourceAmount(type), null, false);
            newElement.gameObject.SetActive(false);

            xPos = newElement.transform.localPosition.x;
            
            UIElements.Add(type, newElement);
        }
    }

    void checkResource(ResourceType type, int amnt)
    {
        if (UIElements.TryGetValue(type, out ElementUI UI))
        {
            if (UI.getAmount() > 0 && !UI.gameObject.activeSelf) //shows the UI element when the value is greater than 0 and it was not active; essentially adds the element to the UI when it's "discovered" for the first time
            {
                UI.gameObject.SetActive(true);
                if (!showingPanel)
                {
                    enqueueNotify(() => notify(type));
                }
            }
            UI.setAmount(amnt, "set");
        }
    }

    void showPanel() {
        animPlaying = true;
        showingPanel = true;

        foreach (ElementUI UI in UIElements.Values)
        {
            //UIXPosition = UI.transform.position.x;
            UI.transform.Translate(Vector3.left * 350);
        }

        UITransformParent.DOMoveX(175, 0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            float i = 0;
            foreach (ElementUI UI in UIElements.Values)
            {
                UI.transform.DOMoveX(220, 0.25f).SetEase(Ease.OutQuad).SetDelay(i);
                i += 0.1f;
            }
        });

        tabText.enabled = true;

        animPlaying = false;

        /*foreach (ElementUI UI in UIElements.Values)
        {
            UI.transform.DOMoveX(-50, 2f).From();
        }*/
    }

    void hidePanel()
    {
        animPlaying = true;
        showingPanel = false;



        float i = 0;
        float count = 0;
        foreach (ElementUI UI in UIElements.Values)
        {
            UI.transform.DOMoveX(-220, 0.25f).SetDelay(i).OnComplete(() =>
            {
                count++;
                if (count == UIElements.Count)
                {
                    UITransformParent.DOMoveX(-175, 0.55f).OnComplete(() => {
                        foreach (ElementUI UI in UIElements.Values)
                        {
                            //UIXPosition = UI.transform.position.x;
                            UI.transform.position = new Vector3(xPos, UI.transform.position.y, UI.transform.position.z);
                        }
                        animPlaying = false;
                        tabText.enabled = true;
                        showNextNotify();
                    });
                }
            });
            i += 0.1f;
            
        }


        
        /* UITransformParent.DOMoveX(-175, 0.75f).OnComplete(() =>
         {
             foreach (ElementUI UI in UIElements.Values)
             {
                 //UIXPosition = UI.transform.position.x;
                 UI.transform.position = new Vector3(xPos, UI.transform.position.y, UI.transform.position.z);
             }
         });*/
    }

    void notify(ResourceType type)
    {
        Debug.Log("notifying");
        ElementUI newElement = Instantiate(UIPrefab, UITransformParent);
        newElement.Load(type, 0, "NEW!", true);

        Sequence notify = DOTween.Sequence();

        notify.Append(newElement.transform.DOMoveX(225, 0.55f).SetEase(Ease.OutQuad));

        notify.AppendInterval(5f);

        notify.Append(newElement.transform.DOMoveX(-395, 0.55f).SetEase(Ease.OutQuad));

        notify.OnComplete(() => {
            Destroy(newElement.gameObject);
            if (!showingPanel)
            {
                showNextNotify();
            }
        });

        
    }

    //AI used below
    public void enqueueNotify(Action method)
    {
        Debug.Log("queued");
      
        notifQueue.Enqueue(method);
        if (!notifying)
        {
            showNextNotify();
        }
    }

    private void showNextNotify()
    {
        if (notifQueue.Count > 0)
        {
            notifying = true;

            Action nextFunction = notifQueue.Dequeue();

            nextFunction.Invoke();
        }
        else
        {
            notifying = false;
        }
    }

}
