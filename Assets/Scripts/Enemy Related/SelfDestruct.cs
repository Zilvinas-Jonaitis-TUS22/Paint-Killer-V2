using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public ParticleSystem bling;
    private Vector3 initialScale;
    public AudioSource AudioSource;
    public AudioClip basicSound;
    public AudioClip rangedSound;
    public string enemyType;
  

    private void Start()
    {
        
    }
    public void EnemyTypeAudio(string enemyType)
    {
        if (enemyType == "Basic")
        {
            AudioSource.clip = basicSound;
        } else if (enemyType== "Ranged")
        {
            AudioSource.clip=rangedSound;
        }
        AudioSource.Play();
    }
    public void SelfDestroy()
    {
       
        Destroy(gameObject);
    }
    public void Bling()
    {
        bling.Play();
    }
    public void SetInitialScale(Vector3 scale)
    {
        initialScale = scale;
    }
    private IEnumerator ShrinkCoroutine()
    {
        float shrinkDuration = 10f; // Adjusted duration for slower shrink
        float timeElapsed = 0;

        while (timeElapsed < shrinkDuration)
        {
            float progress = timeElapsed / shrinkDuration;
            float smoothProgress = Mathf.SmoothStep(0, 1, progress); // Smooth transition
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, smoothProgress);
            timeElapsed += Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
        }
        transform.localScale = Vector3.zero;
        
    }

}

