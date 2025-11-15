using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;   

public class PotionBoard : MonoBehaviour
{
    public int width = 6;
    public int height = 8;

    public float spacingX;
    public float spacingY;

    public GameObject[] potionPrefabs;

    public Node[,] potionBoard;
    public GameObject potionBoardGo;

    public List<GameObject> potionsToDestroy = new();

    public ArrayLayout arrayLayout;
    
    public static PotionBoard instance;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializeBoard();
    }

    void InitializeBoard()
    {
        DestroyPotions();   
        potionBoard = new Node[width, height];

        spacingX = (float)(width - 1) / 2;
        spacingY = (float)((height - 1) / 2) + 1;

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);

                if (arrayLayout.rows[y].row[x])
                {
                    potionBoard[x, y] = new Node(false, null);
                } else
                {
                    int randomIndex = Random.Range(0, potionPrefabs.Length);    
                    GameObject potion = Instantiate(potionPrefabs[randomIndex], position, Quaternion.identity);
                    potion.GetComponent<Potion>().SetIndicies(x, y);
                    potionBoard[x, y] = new Node(true, potion);
                    potionsToDestroy.Add(potion);
                }
            }
        }   

        if(checkBoard())
        {
            Debug.Log("Matches found on initial board, reinitializing");
            InitializeBoard();
        } else
        {
            Debug.Log("No matches found on initial board, proceeding");
        }
    }

    private void DestroyPotions()
    {
        if (potionBoardGo != null)
        {
            foreach(GameObject potion in potionsToDestroy)
            {
                Destroy(potion);
            }
            potionsToDestroy.Clear();
        }
    }

    public bool checkBoard()
    {
        Debug.Log("Checking Board for Matches");
        bool hasMatched = false;

        List<Potion> PotionsToRemove = new();

        for(int x = 0; x  < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if (potionBoard[x, y].isUseable)
                {
                    Potion potion = potionBoard[x, y].potion.GetComponent<Potion>();

                    if(!potion.isMatched)
                    {
                        MatchResult matchPotions = isConnected(potion);
                        if(matchPotions.connectedPotions.Count >= 3)
                        {
                            //complex matching 
                            PotionsToRemove.AddRange(matchPotions.connectedPotions);
                            foreach(Potion pot in matchPotions.connectedPotions)
                            {
                                pot.isMatched = true;
                            }
                            hasMatched = true;
                        }
                    }
                }
            }
        }
        return hasMatched;
    }

    //is connected
    MatchResult isConnected(Potion potion)
    {
        List<Potion> connectedPotions = new List<Potion>();
        //MatchResult result = new MatchResult();
        PotionType type = potion.potionType;
        connectedPotions.Add(potion);

        //check right
        CheckDirection(potion, new Vector2Int(1, 0), connectedPotions);
        //check left
        CheckDirection(potion, new Vector2Int(-1, 0), connectedPotions);

        //horizontal match  for 3
        if (connectedPotions.Count == 3)
        {
            Debug.Log("Normal horizontal match" + connectedPotions[0].potionType);
            return new MatchResult { connectedPotions = connectedPotions, direction = MatchedDirection.Horizontal };
        } else if (connectedPotions.Count >= 4)
        {
            Debug.Log("Long horizontal match" + connectedPotions[0].potionType);
            return new MatchResult { connectedPotions = connectedPotions, direction = MatchedDirection.LongHorizontal };
        }
        connectedPotions.Clear();
        connectedPotions.Add(potion);
        //check up
        CheckDirection(potion, new Vector2Int(0, 1), connectedPotions);
        //check down
        CheckDirection(potion, new Vector2Int(0, -1), connectedPotions);

        //vertical match for 3
        if (connectedPotions.Count == 3)
        {
            Debug.Log("Normal vertical match" + connectedPotions[0].potionType);
            return new MatchResult { connectedPotions = connectedPotions, direction = MatchedDirection.Vertical };
        } else if (connectedPotions.Count >= 4)
        {
            Debug.Log("Long vertical match" + connectedPotions[0].potionType);
            return new MatchResult { connectedPotions = connectedPotions, direction = MatchedDirection.LongVertical };
        }else
        {
            return new MatchResult { connectedPotions = new List<Potion>(), direction = MatchedDirection.None };
        }
    }

    void CheckDirection(Potion pot, Vector2Int direction, List<Potion> connectedPotion) 
    {
        PotionType potionType = pot.potionType;
        int x = pot.xIndex + direction.x;
        int y = pot.yIndex + direction.y;

        //check what is within the boundry
        while(x >= 0 && x < width && y >= 0 && y < height)
        {
            if (potionBoard[x, y].isUseable)
            {
                Potion neighbourPotion = potionBoard[x, y].potion.GetComponent<Potion>();
                if (!neighbourPotion.isMatched && neighbourPotion.potionType == potionType)
                {
                    connectedPotion.Add(neighbourPotion);
                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }
}
public class MatchResult
{
    public List<Potion> connectedPotions;
    public MatchedDirection direction;

    //public MatchResult()
    //{
    //    connectedPotions = new List<Potion>();
    //    direction = MatchedDirection.None;
    //}
}

public enum MatchedDirection
{
    Horizontal,
    Vertical,
    LongVertical,
    LongHorizontal,
    SuperMatch,
    None
}