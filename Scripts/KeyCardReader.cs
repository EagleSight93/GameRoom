using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCardReader : MonoBehaviour
{
    public int codListener;
    public float ejectForce;
    GameObject currentCard;
    Transform cardPosition;
    AudioSource speaker;

    private void Start()
    {
        //Subscribe to OnInteraction as a Listener
        GameManager.current.OnInteraction += ReadCard;

        cardPosition = transform.GetChild(1);
        speaker = GetComponent<AudioSource>();

    }

    //Unsubscribe
    void OnDisable()
    {
        GameManager.current.OnInteraction -= ReadCard;
    }


    void ReadCard(int codinteraction, GameObject item)
    {
        //if the event parameter is correct, it means the GO parameter is valid
        if (codinteraction != codListener) return;

        speaker.Play();

        if (currentCard != null)
        {
            //let go of current object
            currentCard.transform.SetParent(GameManager.current.scenario.transform);
            currentCard.GetComponent<Rigidbody>().isKinematic = false;
            currentCard.GetComponent<Collider>().enabled = true;
            //Eject Card
            currentCard.GetComponent<Rigidbody>().AddForce(cardPosition.forward * ejectForce * Random.Range(1f,1.5f));

            //Insert new card
            item.transform.position = cardPosition.position;
            item.transform.rotation = cardPosition.rotation;
            item.transform.SetParent(cardPosition);

            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Collider>().enabled = false;

            currentCard = item;

        }
        else
        {
            //Insert new card
            item.transform.position = cardPosition.position;
            item.transform.rotation = cardPosition.rotation;
            item.transform.SetParent(cardPosition);

            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Collider>().enabled = false;

            currentCard = item;
        }

    }




}
