using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallState
{
    LEFT = 1,       //0001
    RIGHT = 2,      //0010
    TOP = 4,        //0100
    BOT = 8,        //1000
    VISITED = 128   //1000 0000
}
public struct Position
{
    public int x;
    public int y;
}

public struct Neighbour
{
    public Position position;
    public WallState sharedWall;
}

public class MazeRender : MonoBehaviour
{
    public int codListener;
    public Transform dropLocation;

    private Position start;
    private Position end;
    GameObject currentItem;
    WallState[,] maze;
    int size;
    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to OnInteraction as a Listener
        GameManager.current.OnInteraction += BuildMaze;
    }

    //Unsubscribe
    void OnDisable()
    {
        GameManager.current.OnInteraction -= BuildMaze;
    }

    void BuildMaze(int codinteraction, GameObject item)
    {
        //if the event parameter is correct, it means the GO parameter is valid
        if (codinteraction != codListener) return;

        //Disable Physics of item
        item.transform.SetParent(null);//change to scenario
        item.GetComponent<Rigidbody>().isKinematic = true;
        item.GetComponent<Collider>().enabled = false;

        //if there was another maze already
        if (maze != null)
        {
            //disable new piece model
            item.transform.GetChild(0).gameObject.SetActive(false);

            //dissolve previous maze
            StartCoroutine(DissolveMaze(transform.localScale.y, 1.5f));
            
            StartCoroutine(Exchange(item,1.5f));

            //build new maze
            StartCoroutine(Builder(1.5f));

            //maze orientation
            StartCoroutine(RaiseMaze(transform.localScale.y, 1.5f, 1.5f));
            
        }
        else
        {
            //get item data 
            currentItem = item;
            MazeKey data = currentItem.GetComponent<MazeKey>();
            size = data.mazeSize;
            start = new Position { x = 0, y = size - 1 };//start always at the upper corner

            //build new maze
            StartCoroutine(Builder(0f));
            //maze orientation
            StartCoroutine(RaiseMaze(transform.localScale.y, 1.5f,0f));
        }

    }
    
    //Functions for maze
    public void CreateMaze()
    {
        maze = new WallState[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                maze[i, j] = WallState.LEFT | WallState.RIGHT | WallState.TOP | WallState.BOT;
            }
        }

    }

    public void Backtracker()
    {
        System.Random rng = new System.Random();

        maze[start.x, start.y] |= WallState.VISITED;

        Stack<Position> positionsStack = new Stack<Position>();

        positionsStack.Push(start);

        //list to add every dead end and it's step count
        List<(int, Position)> paths = new List<(int, Position)>(); 
        int stepsTaken = 0;

        while (positionsStack.Count > 0)
        {
            Position current = positionsStack.Pop();
            List<Neighbour> scan = CheckUnvisitedNeighbours(current);
            //if found unvisted arround
            if (scan.Count > 0)
            {
                //get current pos back into stack for backtracking
                positionsStack.Push(current);

                //get a random neighbour
                Neighbour randomNeighbour = scan[rng.Next(0,scan.Count)];

                //break the walls between 
                maze[current.x, current.y] &= ~randomNeighbour.sharedWall;
                maze[randomNeighbour.position.x, randomNeighbour.position.y] &= ~InvertState(randomNeighbour.sharedWall);

                //mark this as visited
                maze[randomNeighbour.position.x, randomNeighbour.position.y] |= WallState.VISITED;
                
                //push to chosen neighbor position
                positionsStack.Push(randomNeighbour.position);

                stepsTaken++;

            }
            else
            {
                //add every dead end, backtraking ones are irrelevant as only the highest count will be used
                paths.Add((stepsTaken,current));
                stepsTaken--;
            }
        }

        //Descending order
        paths.Sort((a, b) => a.Item1.CompareTo(b.Item1));
        paths.Reverse();
        //mark end of maze
        
        /*
        for (int i = 0; i < paths.Count; i++)
        {
            print(paths[i].Item1 + ":" + paths[i].Item2);
        }
        */

        end = new Position { x = paths[0].Item2.x, y = paths[0].Item2.y};
    }


    void DrawMaze2D()
    {
        //Enable new piece
        currentItem.transform.GetChild(0).gameObject.SetActive(true);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                WallState cell = maze[i, j];

                Vector3 position = new Vector3(i - (size / 2), 0, j - (size / 2));

                if (cell.HasFlag(WallState.TOP))
                {
                    GameObject newWall = Instantiate(Resources.Load("Prefabs/Wall", typeof(GameObject)) as GameObject, transform);
                    newWall.transform.localPosition = position + (transform.InverseTransformDirection(transform.forward) * 0.5f);

                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    GameObject newWall = Instantiate(Resources.Load("Prefabs/Wall", typeof(GameObject)) as GameObject, transform);
                    newWall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    newWall.transform.localPosition = position - (transform.InverseTransformDirection(transform.right) * 0.5f);
                    
                }
                
                if (i == size - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        GameObject newWall = Instantiate(Resources.Load("Prefabs/Wall", typeof(GameObject)) as GameObject, transform);
                        newWall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        newWall.transform.localPosition = position + (transform.InverseTransformDirection(transform.right) * 0.5f);
                           

                    }
                }
                
                if (j == 0)
                {
                    if (cell.HasFlag(WallState.BOT))
                    {
                        GameObject newWall = Instantiate(Resources.Load("Prefabs/Wall", typeof(GameObject)) as GameObject, transform);
                        newWall.transform.localPosition = position - (transform.InverseTransformDirection(transform.forward) * 0.5f);

                    }
                }
                
                //if at the start of maze
                if (i == start.x && j == start.y)
                {
                    //set player piece
                    currentItem.transform.SetParent(transform);
                    currentItem.transform.localPosition = position;
                    currentItem.transform.localRotation = Quaternion.identity;
                }
                //if at the end of maze(longest path)
                if (i == end.x && j == end.y)
                {
                    //instantiate goal
                    GameObject goal = Instantiate(Resources.Load("Prefabs/MinecraftChest", typeof(GameObject)) as GameObject,transform);
                    goal.transform.localPosition = position;
                    goal.transform.localRotation = Quaternion.Euler(90, 0, 0);




                }

            }
        }

    }


    public WallState InvertState(WallState state)
    {
        switch (state)
        {
            case WallState.BOT: return WallState.TOP;
            case WallState.TOP: return WallState.BOT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.RIGHT: return WallState.LEFT;
            default: return WallState.LEFT;
        }
    }


    public List<Neighbour> CheckUnvisitedNeighbours(Position p)
    {
        List<Neighbour> neighbours = new List<Neighbour>();

        //left wall
        if (p.x > 0)
        {
            //If not visited then add to list
            if (!maze[p.x - 1, p.y].HasFlag(WallState.VISITED))
            {
                neighbours.Add(new Neighbour
                {
                    position = new Position { x = p.x - 1, y = p.y },
                    sharedWall = WallState.LEFT
                });
            }
        }
        //right wall
        if (p.x < size - 1)
        {
            //If not visited then add to list
            if (!maze[p.x + 1, p.y].HasFlag(WallState.VISITED))
            {
                neighbours.Add(new Neighbour
                {
                    position = new Position { x = p.x + 1, y = p.y },
                    sharedWall = WallState.RIGHT
                });
            }
        }

        //bottom wall
        if (p.y > 0)
        {
            //If not visited then add to list
            if (!maze[p.x, p.y - 1].HasFlag(WallState.VISITED))
            {
                neighbours.Add(new Neighbour
                {
                    position = new Position { x = p.x, y = p.y - 1 },
                    sharedWall = WallState.BOT
                });
            }
        }

        //Top wall
        if (p.y < size - 1)
        {
            //If not visited then add to list
            if (!maze[p.x, p.y + 1].HasFlag(WallState.VISITED))
            {
                neighbours.Add(new Neighbour
                {
                    position = new Position { x = p.x, y = p.y + 1 },
                    sharedWall = WallState.TOP
                });
            }
        }

        return neighbours;
    }


    //Coroutines
    private IEnumerator RaiseMaze(float units,float duration,float wait)
    {
        yield return new WaitForSeconds(wait);
        for (float i = duration; i >0; i-=Time.deltaTime)
        {
            transform.position = transform.position + (transform.up * (units * Time.deltaTime));
            yield return null;
        }
    }

    private IEnumerator DissolveMaze(float units, float seconds)
    {        
        for (float i = seconds; i > 0; i -= Time.deltaTime)
        {
            transform.position = transform.position - (transform.up * (units * Time.deltaTime));
            yield return null;
        }

        //delete all walls
        foreach (Transform child in transform)
        {
            if (!child.transform.CompareTag("Pickable"))
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        //transform.localRotation = Quaternion.identity;
    }

    private IEnumerator Exchange(GameObject item, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        //Disable Physics of item
        currentItem.transform.position = dropLocation.position + (dropLocation.transform.up);
        currentItem.transform.SetParent(GameManager.current.scenario.transform);
        currentItem.GetComponent<Rigidbody>().isKinematic = false;
        currentItem.GetComponent<Collider>().enabled = true;

        //get item data 
        currentItem = item;
        MazeKey data = currentItem.GetComponent<MazeKey>();
        size = data.mazeSize;
        start = new Position { x = 0, y = size - 1 };//start always at the upper corner

    }
    private IEnumerator Builder(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        //Build new Maze
        CreateMaze();
        Backtracker();
        DrawMaze2D();
    }

}
