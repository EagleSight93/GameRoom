using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speaker : MonoBehaviour
{
    public int codListener;

    AudioSource speaker;

    void Start()
    {
        //Subscribe to OnInteraction as a Listener
        GameManager.current.OnInteraction += PlayOrStop;
        GameManager.current.OnCommandDisable += Stop;

        speaker = GetComponent<AudioSource>();
    }

    //Unsubscribe
    void OnDisable()
    {
        GameManager.current.OnInteraction -= PlayOrStop;
        GameManager.current.OnCommandDisable -= Stop;
    }

    void PlayOrStop(int codinteraction, GameObject item)
    {
        
        //if the event parameter is correct, it means the GO parameter is valid
        if (codinteraction != codListener) return;

        if (speaker.isPlaying)
        {
            speaker.Stop();
        }
        else
        {
            speaker.Play();
        }
    }

    void Stop()
    {
        speaker.Stop();
    }

}
