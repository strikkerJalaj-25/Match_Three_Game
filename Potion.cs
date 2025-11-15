using UnityEngine;

public class Potion : MonoBehaviour
{
    public PotionType potionType;

    public int xIndex;
    public int yIndex;

    public bool isMatched;
    private Vector2 currPos;
    private Vector2 targetPos;

    public bool isMoving;

    public Potion(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }
    public void SetIndicies(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    //movetoTarget

    // move coroutine
}

public enum PotionType
{
    Red,
    Blue,
    Green,
    Purple,
    white
}   
