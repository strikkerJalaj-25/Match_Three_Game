using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isUseable;

    public GameObject potion;

    public Node(bool isUseable, GameObject potion)
    {
        this.isUseable = isUseable;
        this.potion = potion;
    }


}
