using UnityEngine;
[System.Serializable]
public class Node
{
    public static float MaxCost = 99f;
    public static float MinCost = 0.5f;
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

            _cost = value;
            _cost = Mathf.Clamp(_cost, MinCost, MaxCost);
        }
    }
    public float defenseBonus = 0;
    public float attackBonus = 0;

    public float g { get; set; }
    public float h { get; set; }
    /// <summary>
    /// The node's g cost and h cost.
    /// </summary>
    /// <returns></returns>
    public float f
    {
        get { return h + g; }
    }
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


    public void UpdateStatus(string name, float cost, bool walkable, int team = 0)
    {
        this.name = name;
        this.cost = cost;
        this.walkable = walkable;
        this.team = team;
    }



}
