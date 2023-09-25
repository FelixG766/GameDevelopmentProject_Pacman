using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource introAudio;
    [SerializeField] private AudioSource normalAudio;

    // Start is called before the first frame update
    void Start()
    {
        introAudio.Play();
        StartCoroutine(PlayBGAfterIntroFinish());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator PlayBGAfterIntroFinish(){

        while (introAudio.isPlaying){
            yield return null;
        }

        normalAudio.loop = true;
        normalAudio.Play();
    }
}
