using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool requiresItem;
    public int codInteractable;

    //Start event on GameManager passing the event and the picked item if they have the same code
    public bool Interact(GameObject item)
    {
        if (requiresItem)
        {
            if (item.GetComponent<Pickable>().codPickable == codInteractable)
            {
                GameManager.current.ExecuteInteraction(codInteractable, item);
                return true;
            }
        }
        else
        {
            GameManager.current.ExecuteInteraction(codInteractable, null);
            return true;
        }


        return false;
    }
  
}
