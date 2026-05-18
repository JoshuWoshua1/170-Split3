using UnityEngine;
using UnityEngine.Playables;

public class CutsceneScript : MonoBehaviour
{

    [SerializeField] PlayerController player;
    [SerializeField] PlayableDirector cutscene;
    [SerializeField] int sizeToFinish = 4;
    [SerializeField] AudioSource muteAudioSource;


    // Update is called once per frame
    void Update()
    {
        if (player.size >= sizeToFinish)
        {
            if (cutscene != null)
            {
                cutscene.Play();
            }
            if (muteAudioSource != null)
            {
                muteAudioSource.mute = true;
            }
        }
    }
}
