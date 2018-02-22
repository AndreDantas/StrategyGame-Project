using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{

    /// <summary>
    /// If the character is moving.
    /// </summary>
    protected bool moving;


    [SerializeField]
    int _currentHp;
    public int currentHp
    {
        get { return _currentHp; }
        set
        {
            if (value > maxHp)
                _currentHp = maxHp;
            else
                _currentHp = maxHp;
        }
    }

    [SerializeField]
    int _maxHp;
    public int maxHp
    {
        get { return _maxHp; }
        set
        {
            if (value <= 0)
                _maxHp = 1;
            else
                _maxHp = value;
        }
    }
    private void OnValidate()
    {
        //TEST
        transform.position = new Vector2(x + 0.5f, y + 0.5f);
    }
    private void Start()
    {

        InitializeOnMap();
        //foreach (Node n in FindRange(x, y, 1))
        //{
        //    print("Node " + n.name + ": " + n.x + ", " + n.y);
        //}
        //foreach (Node n in ExpandArea(FindRange(x, y, 1), 1))
        //{
        //    print("Node " + n.name + ": " + n.x + ", " + n.y);
        //}

    }
    private void Update()
    {

    }

    /// <summary>
    /// Initializes the unit on the map.
    /// </summary>
    public void InitializeOnMap()
    {

        if (map == null)
            return;

        transform.position = new Vector2(x + map.nodeOffsetX, y + map.nodeOffsetY);
        if (map.ValidCoordinate(x, y))
        {
            map.nodes[x, y].unitOnNode = this;
        }

    }

    /// <summary>
    /// Moves the character between nodes.
    /// </summary>
    /// <param name="destination">The destination node.</param>
    /// <param name="duration">How long the movement takes in seconds.</param>
    /// <returns></returns>
    protected IEnumerator LerpMove(Node destination, float duration = 0.15f)
    {
        if (moving || destination == null || map == null)
            yield break;
        if (destination.x == x && destination.y == y)
            yield break;
        if (!ValidNode(destination) || !map.ValidCoordinate(destination))
            yield break;
        float t = 0;
        Vector2 start = new Vector2(x + map.nodeOffsetX, y + map.nodeOffsetY);
        Vector2 end = new Vector2(destination.x + map.nodeOffsetX, destination.y + map.nodeOffsetY);
        float lerpTime = 0f;
        moving = true;
        while (t < 1)
        {
            t = lerpTime / duration;
            transform.position = Vector3.Lerp(start, end, t);
            lerpTime += Time.deltaTime;
            yield return null;
        }
        map.nodes[x, y].unitOnNode = null;
        x = destination.x;
        y = destination.y;
        map.nodes[x, y].unitOnNode = this;
        moving = false;
    }

    /// <summary>
    /// Moves the character on a path.
    /// </summary>
    /// <param name="path">The list of nodes that makes the path</param>
    public virtual void WalkPath(List<Node> path)
    {
        if (path == null)
            return;
        StartCoroutine(WalkingPath(path));
    }

    protected IEnumerator WalkingPath(List<Node> path)
    {
        if (map == null || path == null)
            yield break;
        if (moving)
            yield break;
        foreach (Node n in path)
        {
            yield return LerpMove(n);
        }
    }

    /// <summary>
    /// A* path find algorithm. 
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    public virtual List<Node> PathFind(Node destination)
    {
        // Link to algorithm: https://en.wikipedia.org/wiki/A*_search_algorithm

        if (!map) // The map can't be null.
            return null;
        if ((destination = validDestination(destination)) == null) // The destination node has to be valid.
            return null;
        if (!map.ValidCoordinate(x, y)) // The start node as to be valid.
            return null;

        bool found = false;

        // Each node's g cost starts with the default value of infinity.
        foreach (Node n in map.GetNodes())
        {
            n.g = Node.MaxCost;
            n.parent = null;
        }

        // Starting node
        Node start = map.nodes[x, y];
        start.g = 0;
        start.h = Map.Distance(start, destination, NodeDistance.Manhattan);

        List<Node> open = new List<Node>(); // Open set
        List<Node> closed = new List<Node>(); // Closed set

        open.Add(start);
        Node current;
        while (open.Count > 0)
        {

            int lowestf = 0;
            for (int i = 0; i < open.Count; i++)
            {
                if (open[i].f < open[lowestf].f)
                    lowestf = i;
            }
            current = open[lowestf];

            if (current.x == destination.x && current.y == destination.y)
            {
                found = true;
                closed.Add(current);
                break;
            }


            open.Remove(current);
            closed.Add(current);

            List<Node> neighbors = map.GetNeighbors(current);
            foreach (Node n in neighbors)
            {

                if (closed.Contains(n))
                    continue;
                if (!ValidNode(n))
                    continue;


                float gscore = current.g + NodeCostEvaluation(n);
                bool newPath = false;
                if (open.Contains(n))
                {
                    if (gscore < n.g)
                    {
                        n.g = gscore;
                        newPath = true;
                    }
                }
                else
                {
                    n.g = gscore;
                    open.Add(n);
                    newPath = true;
                }

                if (newPath)
                {
                    n.parent = current;
                    n.h = Map.Distance(n, destination, NodeDistance.Manhattan);
                }


            }
        }
        if (found)
        {
            return ReconstructPath(destination);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns a list of reachable nodes based on range. Use it to find all nodes a character can reach.
    /// </summary>
    /// <param name="range">The max range.</param>
    /// <returns></returns>
    public List<Node> FindRange(int x, int y, int range)
    {
        // Based on Dijkstra's Algorithm: https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm

        if (!map) // The map can't be null.
            return null;
        if (!map.ValidCoordinate(x, y))
            return null;

        range = MathOperations.ClampMin(range, 0);
        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();

        foreach (Node n in map.GetNodes())
        {
            n.g = 0f;
        }

        Node current;
        openSet.Add(map.nodes[x, y]);

        while (openSet.Count > 0)
        {
            current = openSet[0];
            closedSet.Add(current);
            openSet.RemoveAt(0);

            if (current.g <= range)
            {
                foreach (Node neighbor in map.GetNeighbors(current))
                {
                    if (!ValidNode(neighbor))
                        continue;
                    float newG = current.g + NodeCostEvaluation(neighbor);
                    if (neighbor.g == 0 || newG < neighbor.g)
                    {
                        neighbor.g = newG;
                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }
        }
        List<Node> result = new List<Node>();
        foreach (Node n in closedSet)
        {
            if (n.g <= range)
                result.Add(n);
        }
        return result;
    }

    /// <summary>
    /// Used to extend an area by a range. Can be used to find the attack range of a character based on his move area.
    /// </summary>
    /// <param name="baseArea">The area to be expanded.</param>
    /// <param name="range">The range of the expansion.</param>
    /// <returns>This function doesn't return all nodes, only the new ones.</returns>
    public List<Node> ExpandArea(List<Node> baseArea, int range)
    {
        if (!map) // The map can't be null.
            return null;
        int[,] rangeMap = map.Manhattan(baseArea);
        List<Node> newNodes = new List<Node>();
        for (int i = 0; i < rangeMap.GetLength(0); i++)
        {
            for (int j = 0; j < rangeMap.GetLength(1); j++)
            {
                if (rangeMap[i, j] <= range && rangeMap[i, j] > 0)
                    newNodes.Add(map.nodes[i, j]);

            }
        }
        return newNodes;
    }


    protected static List<Node> ReconstructPath(Node n)
    {
        if (n == null)
            return null;
        List<Node> path = new List<Node>();
        Node current = n;
        path.Add(current);
        while (current.parent != null)
        {
            current = current.parent;
            path.Add(current);

        }
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Returns true if node can be used as path.
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public virtual bool ValidNode(Node n)
    {
        // Each different character should have a unique way to validate nodes
        return n.walkable && n.unitOnNode == null;
    }

    /// <summary>
    /// Checks if destination is valid.
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public virtual Node validDestination(Node n)
    {
        // Can be used to find a new destination and return the new node.
        if (ValidNode(n) && map.ValidCoordinate(n))
        {
            return n;
        }
        else return null;
    }

    /// <summary>
    /// Returns the new cost for the node.
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public virtual float NodeCostEvaluation(Node n)
    {
        // Can be used to change the node initial walk cost.
        return n.cost;
    }
}
