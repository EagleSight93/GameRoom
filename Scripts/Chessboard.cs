using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    public int codListener;
    
    GameObject board;
    Dictionary<char, string> pieces;
    string positions;
    int xPos;
    int yPos;

    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to OnInteraction as a Listener
        GameManager.current.OnInteraction += DrawChessBoard;

        //get board
        board = transform.GetChild(1).gameObject;

        //dictionary of pieces
        pieces = new Dictionary<char, string>()
        {
            {'p', "BlackPawn"},
            {'r', "BlackRook"},
            {'n', "BlackKnight"},
            {'b', "BlackBishop"},
            {'q', "BlackQueen"},
            {'k', "BlackKing"},
            {'P', "WhitePawn"},
            {'R', "WhiteRook"},
            {'N', "WhiteKnight"},
            {'B', "WhiteBishop"},
            {'Q', "WhiteQueen"},
            {'K', "WhiteKing"}
        };

        xPos = 0;
        yPos = 0;

    }
    //Unsubscribe
    void OnDisable()
    {
        GameManager.current.OnInteraction -= DrawChessBoard;
    }

    void DrawChessBoard(int codinteraction,GameObject item)
    {
        //if the event parameter is correct, it means the GO parameter is valid
        if (codinteraction != codListener) return;

        positions = item.transform.GetChild(1).GetComponent<TextMeshPro>().text;

        // check if board
        if (board == null) return;

        //Clean board and reset x,y positions
        foreach (Transform child in board.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        xPos = 0;
        yPos = 0;

        // read positions string
        for (int i = 0; i < positions.Length; i++)
        {
            char piece = positions[i];
            //ignore everything over 8th row
            if (yPos < 8)
            {
                if (char.IsLetterOrDigit(piece))
                {
                    if (char.IsLetter(piece))
                    {
                        if (xPos < 8)
                        {
                            //print("Spawn Chesspiece");
                            if (pieces.ContainsKey(piece))
                            {
                                GameObject newPiece = Instantiate(Resources.Load("Prefabs/Chess/" + pieces[piece], typeof(GameObject)) as GameObject, board.transform);
                                newPiece.transform.localPosition = new Vector3(xPos + (0.5f * xPos), 0, yPos + (0.5f * yPos));
                                newPiece.transform.localRotation = char.IsUpper(piece) ? Quaternion.identity : Quaternion.Euler(newPiece.transform.parent.localRotation.x, 180, newPiece.transform.parent.localRotation.z);
                                xPos++;
                            }
                            else
                            {
                                //print("Skip this cell");
                                xPos++;
                            }

                        }
                    }
                    else
                    {
                        for (int a = 0; a < char.GetNumericValue(piece); a++)
                        {
                            if (xPos < 8)
                            {
                                //print("Skip this cell");
                                xPos++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                }
                else
                {
                    if (piece.Equals('/'))
                    {
                        do
                        {
                            if (xPos < 8)
                            {
                                //print("Skip this cell");
                                xPos++;
                            }
                            else
                            {
                                //print("Jump Row");
                                xPos = 0;
                                yPos++;
                                break;
                            }
                        } while (true);
                    }
                 
                    //Ignore every other special character
                }

            }

        }

    }
}
