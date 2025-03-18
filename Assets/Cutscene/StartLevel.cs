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

    void OnTimelineFinished(PlayableDirector director)
    {
        if (director == timeline)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
