using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnigmaChyperMachine : MonoBehaviour
{
    public int codListener;
    public float ejectForce;
    
    
    string freqLang;
    string msgfrequency;
    string codedText;
    string decodedText;
    Transform printPosition;
    bool printing;

    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to OnInteraction as a Listener
        GameManager.current.OnInteraction += Decrypt;

        printPosition = transform.GetChild(1);
        printing = false;
        freqLang = "TEOAISRHNUCMDLGWFPYKJBVQX";
    }

    //Unsubscribe
    void OnDisable()
    {
        GameManager.current.OnInteraction -= Decrypt;
    }

    void Decrypt(int codinteraction, GameObject item)
    {
        //if the event parameter is correct, it means the GO parameter is valid
        if (codinteraction != codListener) return;

        if (!printing)
        {
            //disable physics
            item.transform.SetParent(null);//change to scenario
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Collider>().enabled = false;
            codedText = item.transform.GetChild(2).GetComponent<TextMeshPro>().text;

            //disable item
            item.SetActive(false);

            //decrypt text
            decodedText = DecryptMessage(codedText, freqLang);
            //enable and replace text
            item.SetActive(true);
            item.transform.GetChild(2).GetComponent<TextMeshPro>().text = decodedText;

            item.transform.position = printPosition.position;
            item.transform.rotation = printPosition.rotation;

            //Print Animation for page
            StartCoroutine(PrintAnwser(item));
        }
        else
        {
            //enable physics and let go
            item.transform.SetParent(GameManager.current.scenario.transform);
            item.GetComponent<Rigidbody>().isKinematic = false;
            item.GetComponent<Collider>().enabled = true;
        }

    }

    string DecryptMessage(string message, string freqLang)
    {

        msgfrequency = "";
        decodedText = "";
        //count of letters;

        int char_A = 0;
        int char_B = 0;
        int char_C = 0;
        int char_D = 0;
        int char_E = 0;
        int char_F = 0;
        int char_G = 0;
        int char_H = 0;
        int char_I = 0;
        int char_J = 0;
        int char_K = 0;
        int char_L = 0;
        int char_M = 0;
        int char_N = 0;
        int char_O = 0;
        int char_P = 0;
        int char_Q = 0;
        int char_R = 0;
        int char_S = 0;
        int char_T = 0;
        int char_U = 0;
        int char_V = 0;
        int char_W = 0;
        int char_X = 0;
        int char_Y = 0;
        int char_Z = 0;

        //create a list to organize them by frequency
        List<(int,char)> frecuencies = new List<(int,char)>();

        foreach (char c in message){
               
            switch (char.ToUpper(c))
            {
                case 'A': char_A++; break;
                case 'B': char_B++; break;
                case 'C': char_C++; break;
                case 'D': char_D++; break;
                case 'E': char_E++; break;
                case 'F': char_F++; break;
                case 'G': char_G++; break;
                case 'H': char_H++; break;
                case 'I': char_I++; break;
                case 'J': char_J++; break;
                case 'K': char_K++; break;
                case 'L': char_L++; break;
                case 'M': char_M++; break;
                case 'N': char_N++; break;
                case 'O': char_O++; break;
                case 'P': char_P++; break;
                case 'Q': char_Q++; break;
                case 'R': char_R++; break;
                case 'S': char_S++; break;
                case 'T': char_T++; break;
                case 'U': char_U++; break;
                case 'V': char_V++; break;
                case 'W': char_W++; break;
                case 'X': char_X++; break;
                case 'Y': char_Y++; break;
                case 'Z': char_Z++; break;
            }
        }
        
        frecuencies.Add((char_A,'A'));
        frecuencies.Add((char_B,'B'));
        frecuencies.Add((char_C,'C'));
        frecuencies.Add((char_D,'D'));
        frecuencies.Add((char_E,'E'));
        frecuencies.Add((char_F,'F'));
        frecuencies.Add((char_G,'G'));
        frecuencies.Add((char_H,'H'));
        frecuencies.Add((char_I,'I'));
        frecuencies.Add((char_J,'J'));
        frecuencies.Add((char_K,'K'));
        frecuencies.Add((char_L,'L'));
        frecuencies.Add((char_M,'M'));
        frecuencies.Add((char_N,'N'));
        frecuencies.Add((char_O,'O'));
        frecuencies.Add((char_P,'P'));
        frecuencies.Add((char_Q,'Q'));
        frecuencies.Add((char_R,'R'));
        frecuencies.Add((char_S,'S'));
        frecuencies.Add((char_T,'T'));
        frecuencies.Add((char_U,'U'));
        frecuencies.Add((char_V,'V'));
        frecuencies.Add((char_W,'W'));
        frecuencies.Add((char_X,'X'));
        frecuencies.Add((char_Y,'Y'));
        frecuencies.Add((char_Z,'Z'));

        //order list and reverse it
        frecuencies.Sort();
        frecuencies.Reverse();

        //create this message frecuency string
        foreach ((int,char) i in frecuencies)
        {
            //print(i.Item1 + " : " + i.Item2);
            msgfrequency += i.Item2;
        }

        //print(msgfrequency);

        //replace chars
        foreach (char c in message)
        {
            for(int i = 0;i <= 26; i++)
            {
                if(i == 26)
                {
                    decodedText += c;
                    break;
                }

                if (char.ToUpper(c).Equals(msgfrequency[i]))
                {
                    decodedText += char.IsUpper(c) ? freqLang[i] : char.ToLower(freqLang[i]);
                    break;
                }
            }
        }

        return decodedText;
    }


    IEnumerator PrintAnwser(GameObject paper)
    {
        printing = true;
        transform.GetComponent<AudioSource>().Play();

        for (int i = 0;i < 4; i++)
        {
            for (float ft = 0.1375f; ft >= 0; ft -= 0.001375f)
            {
                paper.transform.position = paper.transform.position + new Vector3(0, 0.001375f, 0);
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.25f);
        }
        
        transform.GetComponent<AudioSource>().Stop();

        //enable physics
        paper.transform.SetParent(GameManager.current.scenario.transform);
        paper.GetComponent<Rigidbody>().isKinematic = false;
        paper.GetComponent<Collider>().enabled = true;
        //add force to paper
        paper.GetComponent<Rigidbody>().AddForce(printPosition.up * ejectForce/2 * Random.Range(1f, 1.5f));
        paper.GetComponent<Rigidbody>().AddForce(printPosition.forward * ejectForce * Random.Range(1f, 1.5f));

        printing = false;


    }


}
