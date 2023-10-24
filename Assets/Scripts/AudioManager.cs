using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource introAudio;
    [SerializeField] private AudioSource normalAudio;
    [SerializeField] private AudioSource scaredAudio;
    [SerializeField] private AudioSource ghostDeadAudio;
    [SerializeField] private AudioSource countDownAudio;
    
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            Debug.Log("Triggered");
            normalAudio.Stop();
            introAudio.Stop();
            introAudio.loop = true;
            introAudio.Play();
        }
        if (scene.buildIndex == 1)
        {
            introAudio.Stop();
        }
    }

    public void PlayCountDownAudio()
    {
        if (countDownAudio.isPlaying)
        {
            countDownAudio.Stop();
        }
        countDownAudio.Play();
    }

    public void PlayScaredAudio()
    {
        if (!scaredAudio.isPlaying)
        {
            introAudio.Stop();
            normalAudio.Stop();
            scaredAudio.Play();
            ghostDeadAudio.Stop();
        }
    }

    public void PlayGhostDeadAudio()
    {
        if (!ghostDeadAudio.isPlaying)
        {
            introAudio.Stop();
            normalAudio.Stop();
            scaredAudio.Stop();
            ghostDeadAudio.Play();
        }
    }

    public void ResumeNormalAudio()
    {
        if (!normalAudio.isPlaying)
        {
            introAudio.Stop();
            normalAudio.Play();
            scaredAudio.Stop();
            ghostDeadAudio.Stop();
        }
    }

}
