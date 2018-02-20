using UnityEngine;
[System.Serializable]
public class Node
{
    public static float MaxCost = 99f;
    /// <summary>
    /// The node's name.
    /// </summary>
    public string name;
    /// <summary>
    /// Node's X coordinate on the map.
    /// </summary>
    public int x;
    /// <summary>
    /// Node's Y coordinate on the map.
    /// </summary>
    public int y;

    [SerializeField]
    protected float _cost;
    /// <summary>
    /// Node's walk cost.
    /// </summary>
    public float cost
    {
        get { return _cost; }
        set
        {
            if (value > MaxCost)
                _cost = MaxCost;
            else
                _cost = value;
        }
    }
    public float g, h;
    /// <summary>
    /// Unit standing on node.
    /// </summary>
    public Unit unitOnNode;
    /// <summary>
    /// If the node can be walked on.
    /// </summary>
    public bool walkable = true;
    /// <summary>
    /// Used to sort wich team the node belongs.
    /// </summary>
    public int team;
    /// <summary>
    /// Used for pathfind.
    /// </summary>
    public Node parent;

    public Node(int x, int y, string name, float cost, bool walkable)
    {
        this.x = x;
        this.y = y;
        this.name = name;
        this.cost = cost;
        this.walkable = walkable;

    }
    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.cost = 1;
    }
    public Node()
    {
        this.x = 0;
        this.y = 0;
        this.cost = 1;
    }

    public Node(Node other)
    {
        this.name = other.name;
        this.cost = other.cost;
        this.walkable = other.walkable;
        this.x = other.x;
        this.y = other.y;
        this.parent = other.parent;
        this.team = other.team;
        this.unitOnNode = other.unitOnNode;

    }

    /// <summary>
    /// The node's g cost and h cost.
    /// </summary>
    /// <returns></returns>
    public float F()
    {
        return h + g;
    }
    public void UpdateStatus(string name, float cost, bool walkable, int team = 0)
    {
        this.name = name;
        this.cost = cost;
        this.walkable = walkable;
        this.team = team;
    }



}
