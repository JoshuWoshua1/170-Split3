using UnityEngine;
using UnityEngine.Playables;

public class CutsceneScript : MonoBehaviour
{

    [SerializeField] PlayerController player;
    [SerializeField] PlayableDirector cutscene;

    // Update is called once per frame
    void Update()
    {
        if (player.size >= 4)
        {
            if (cutscene != null)
            {
                cutscene.Play();
            }
        }
    }
}
