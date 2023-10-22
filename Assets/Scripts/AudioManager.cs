using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource introAudio;
    [SerializeField] private AudioSource normalAudio;

    void Awake() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        //int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator PlayBGAfterIntroFinish()
    {

        while (introAudio.isPlaying)
        {
            yield return null;
        }

        normalAudio.loop = true;
        normalAudio.Play();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            if (introAudio.isPlaying) 
            {
                introAudio.Stop();
            }
            if (normalAudio.isPlaying) 
            {
                normalAudio.Stop();
            }
            introAudio.loop = true;
            introAudio.Play();
        }
        else if (scene.buildIndex == 1)
        {
            if (introAudio.isPlaying) 
            {
                introAudio.Stop();
            }
            introAudio.loop = false;
            introAudio.Play();
            StartCoroutine(PlayBGAfterIntroFinish());
        }
    }
}
