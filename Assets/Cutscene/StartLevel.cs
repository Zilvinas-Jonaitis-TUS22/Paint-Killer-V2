using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
public class StartLevel : MonoBehaviour
{
    public PlayableDirector timeline;
    public string sceneName;

    void Start()
    {
        if (timeline != null)
        {
            timeline.stopped += OnTimelineFinished;
        }
        else
        {
            Debug.LogError("PlayableDirector not assigned!");
        }
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    void OnTimelineFinished(PlayableDirector director)
    {
        if (director == timeline)
        {
            StartCoroutine(LoadSceneAsync());
        }
    }
}
