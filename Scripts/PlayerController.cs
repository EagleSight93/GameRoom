using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector2 movement;
    float xCamInput;
    float yCamInput;
    float horizontalRotation;
    float camSize;
    float camZoom;
    AudioSource source;
    TextMeshProUGUI description;
    TextMeshProUGUI prompts;
    Transform grabPosition;
    GameObject currentObject;


    public float speed;
    public float camSpeed;
    public float maxGrabDistace;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        horizontalRotation = 0;
        camSize = Camera.main.fieldOfView;
        camZoom = camSize;
        source = transform.GetComponent<AudioSource>();
        description = Camera.main.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        prompts = Camera.main.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        grabPosition = transform.GetChild(0).GetChild(1);
    }

    void Update()
    {
        //Get Inputs
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        xCamInput = Input.GetAxisRaw("Mouse X");
        yCamInput = Input.GetAxisRaw("Mouse Y");

        //play steps if moving
        if (movement == Vector2.zero)
        {
            source.Stop();
        }
        else
        {
            if(!source.isPlaying) source.Play();
        }
        

        //Set cam rotation as a degree
        horizontalRotation -= yCamInput * camSpeed;
        horizontalRotation = Mathf.Clamp(horizontalRotation, -90f, 70f);

        //Movement
        transform.position = transform.position + (transform.forward * (movement.y * speed * Time.deltaTime));
        transform.position = transform.position + (transform.right * (movement.x * speed * Time.deltaTime));

        //Rotation
        transform.Rotate(Vector3.up, xCamInput * camSpeed);
        Camera.main.transform.localRotation = Quaternion.Euler(horizontalRotation, 0, 0);


        //Camera Zoom
        if (Input.GetMouseButton(1))
        {
            camZoom -= 120 * Time.deltaTime;
            camZoom = Mathf.Clamp(camZoom, 20f, 60f);
            Camera.main.fieldOfView = camZoom;
        }

        if (!Input.GetMouseButton(1) && Camera.main.fieldOfView != camSize)
        {
            camZoom += 120 * Time.deltaTime;
            camZoom = Mathf.Clamp(camZoom, 20f, 60f);
            Camera.main.fieldOfView = camZoom;
        }

        //Object description
        
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, maxGrabDistace))
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Pickable") || hit.collider.CompareTag("Interactable")) 
                    description.text = hit.collider.name;
            }
            else
            {
                description.text = "";
            }
        }
        else
        {
            description.text = "";
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Pickable"))
                {
                    if (currentObject == null)
                    {
                        hit.collider.transform.position = grabPosition.position;
                        hit.collider.transform.rotation = grabPosition.rotation;
                        hit.collider.transform.SetParent(grabPosition);

                        hit.collider.GetComponent<Rigidbody>().isKinematic = true;
                        hit.collider.enabled = false;
                        currentObject = hit.collider.gameObject;
                    }
                    else
                    {
                        //let go of current object
                        currentObject.transform.SetParent(GameManager.current.scenario.transform);//change to scenario
                        currentObject.GetComponent<Rigidbody>().isKinematic = false;
                        currentObject.GetComponent<Collider>().enabled = true;

                        //grab something else
                        hit.collider.transform.position = grabPosition.position;
                        hit.collider.transform.rotation = grabPosition.rotation;
                        hit.collider.transform.SetParent(grabPosition);

                        hit.collider.GetComponent<Rigidbody>().isKinematic = true;
                        hit.collider.enabled = false;
                        currentObject = hit.collider.gameObject;
                    }
                }
                else if (hit.collider.CompareTag("Interactable"))
                {
                    if (hit.collider.GetComponent<Interactable>().requiresItem)
                    {
                        if (currentObject != null)
                        {
                            if (hit.collider.GetComponent<Interactable>().Interact(currentObject))
                            {
                                currentObject = null;
                            }
                            else
                            {
                                StartCoroutine(ShowOnPrompt(1.5f,"This item is incompatible"));
                            }
                        }
                        else
                        {
                            StartCoroutine(ShowOnPrompt(1.5f, "Needs an item to work"));
                        }
                    }
                    else
                    {
                        //item doesn't require item so send null;
                        hit.collider.GetComponent<Interactable>().Interact(null);
                    }
                  
                }
                else
                {
                    if (currentObject != null)
                    {
                        //let go of current object
                        currentObject.transform.SetParent(GameManager.current.scenario.transform);//change to scenario
                        currentObject.GetComponent<Rigidbody>().isKinematic = false;
                        currentObject.GetComponent<Collider>().enabled = true;
                        currentObject = null;
                    }
                }
            }
            else
            {
                if (currentObject != null)
                {
                    //let go of current object
                    currentObject.transform.SetParent(GameManager.current.scenario.transform);//change to scenario
                    currentObject.GetComponent<Rigidbody>().isKinematic = false;
                    currentObject.GetComponent<Collider>().enabled = true;
                    currentObject = null;
                }
            }
        }

    }


    private IEnumerator ShowOnPrompt(float duration,string message)
    {
        for (float i = duration; i > 0; i -= Time.deltaTime)
        {
            prompts.text = message;
            yield return null;
        }

        prompts.text = "";

    }





}
