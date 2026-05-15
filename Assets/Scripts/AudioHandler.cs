using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] AudioSource walkingAudioSource;
    private PlayerController player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var audioSources = GetComponents<AudioSource>();
        walkingAudioSource = audioSources[1];
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.IsMoving)
        {
            if (!walkingAudioSource.isPlaying)
                walkingAudioSource.Play();
        }
        else if (walkingAudioSource.isPlaying)
        {
            walkingAudioSource.Pause();
        }
    }
}
