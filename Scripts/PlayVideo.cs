using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{

    public int codListener;
    VideoPlayer player;

    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to OnInteraction as a Listener
        GameManager.current.OnInteraction += PlayOrStopVideo;

        player = GetComponent<VideoPlayer>();
    }

    void PlayOrStopVideo(int codinteraction, GameObject item)
    {
        
        //if the event parameter is correct, it means the GO parameter is valid
        if (codinteraction != codListener) return;
        
        if (player.isPlaying)
        {
            player.Stop();
            player.targetTexture.Release();
        }
        else
        {
            GameManager.current.StopAmbientSounds();
            player.Play();
        }
    }
}
