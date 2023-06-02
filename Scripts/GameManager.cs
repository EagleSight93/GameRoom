using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages Events
public class GameManager : MonoBehaviour
{
    //declare singleton
    public static GameManager current;
    public GameObject scenario;

    //Create Action event
    public event Action<int, GameObject> OnInteraction;
    public event Action OnCommandDisable;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        //declare scenario where pickables will be dropped
        scenario = GameObject.Find("Room").transform.GetChild(0).gameObject;
    }

    public void ExecuteInteraction(int codInteractable, GameObject interactable)
    {
        if (OnInteraction != null) OnInteraction(codInteractable, interactable);
    }

    public void StopAmbientSounds()
    {       
        if (OnCommandDisable != null) OnCommandDisable();
    }



    //ESC to quit
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }



}
