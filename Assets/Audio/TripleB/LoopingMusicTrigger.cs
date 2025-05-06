using UnityEngine;

public class LoopingMusicTrigger : MonoBehaviour
{
    public AudioSource musicSource; // Assign an AudioSource with the looped music
    public string playerTag = "Player";

    private void Start()
    {
        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }
}
