using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{

    /// <summary>
    /// If the character is moving.
    /// </summary>
    protected bool moving;
    public Stat healthBar = new Stat();
    Canvas healthUI;
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
                _currentHp = value;
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

    [SerializeField]
    float _currentStamina;
    public float currentStamina
    {
        get { return _currentStamina; }
        set
        {
            if (value > maxStamina)
                _currentStamina = maxStamina;
            else
                _currentStamina = value;
        }
    }

    [SerializeField]
    float _maxStamina;
    public float maxStamina
    {
        get { return _maxStamina; }
        set
        {
            if (value <= 0)
                _maxStamina = 1;
            else
                _maxStamina = value;
        }
    }
    [SerializeField]
    int _attack = 3;
    public int attack
    {
        get
        {
            return _attack;
        }
        set
        {
            if (value < 0)
                _attack = 0;
            else
                _attack = value;
        }
    }
    [SerializeField]
    int _defense = 1;
    public int defense
    {
        get
        {
            return _defense;
        }
        set
        {
            if (value < 0)
                _defense = 0;
            else
                _defense = value;
        }
    }

    public int attackRange = 1;

    public AreaRangeRenderer walkRangeRenderer;
    public AreaRangeRenderer attackRangeRenderer;

    private void OnValidate()
    {
        //TEST
        transform.position = new Vector2(x + 0.5f, y + 0.5f);

    }
    private void Start()
    {
        InitializeOnMap();
        if (healthBar.GetBar() == null)
            healthBar.SetBar(GetComponentInChildren<Bar>());
        healthBar.MaxValue = maxHp;
        healthBar.CurrentValue = currentHp;
        healthUI = GetComponentInChildren<Canvas>();
        ShowHealthBar();
    }

    public void ShowHealthBar()
    {
        if (healthUI)
        {
            healthUI.gameObject.SetActive(true);
            healthUI.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0.6f);
        }
    }
    public void HideHealthBar()
    {
        if (healthUI)
        {
            healthUI.gameObject.SetActive(false);
        }
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

        currentHp = maxHp;
        currentStamina = maxStamina;

    }

    public void Place(int x, int y)
    {
        if (map == null)
            return;
        if (map.ValidCoordinate(x, y) ? map.nodes[x, y].unitOnNode == null && map.nodes[x, y].walkable == true : false)
        {
            if (map.ValidCoordinate(this.x, this.y) ? (Character)map.nodes[this.x, this.y].unitOnNode == this : false)
            {
                map.nodes[this.x, this.y].unitOnNode = null;
            }
            this.x = x;
            this.y = y;
            map.nodes[x, y].unitOnNode = this;
            transform.position = new Vector2(x + map.nodeOffsetX, y + map.nodeOffsetY);
        }
    }

    public bool InRange(Node n)
    {
        return Map.DefaultManhattanDistance(n, new Node(x, y)) <= attackRange;
    }

    public virtual void StartTurn()
    {
        currentStamina = maxStamina;
    }
    public virtual void Damage(int damage)
    {
        int newDamage = MathOperations.ClampMin(damage - defense, 0);
        currentHp -= newDamage;
        if (currentHp <= 0) // Character dies
            currentHp = 0;
        healthBar.CurrentValue = currentHp;
    }

    #region Movement
    /// <summary>
    /// Moves the character between nodes.
    /// </summary>
    /// <param name="destination">The destination node.</param>
    /// <param name="duration">How long the movement takes in seconds.</param>
    /// <returns></returns>
    protected IEnumerator LerpMove(Node destination, float duration = 0.15f)
    {
        if (destination == null || map == null)
            yield break;
        if (destination.x == x && destination.y == y)
            yield break;
        if (!ValidNode(destination) || !map.ValidCoordinate(destination))
            yield break;
        float t = 0;
        Vector2 start = new Vector2(x + map.nodeOffsetX, y + map.nodeOffsetY);
        Vector2 end = new Vector2(destination.x + map.nodeOffsetX, destination.y + map.nodeOffsetY);
        float lerpTime = 0f;

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

        // TEST
        if (GetPathCost(path) > currentStamina)
            yield break;
        ClearAttackRange();
        ClearWalkRange();
        ////
        moving = true;
        foreach (Node n in path)
        {
            yield return LerpMove(n);
        }
        moving = false;
    }
    /// <summary>
    /// If the character is moving.
    /// </summary>
    /// <returns></returns>
    public bool IsMoving()
    {
        return moving;
    }
    #endregion
    #region Pathfinding and AreaRange
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

        // Each node's g cost starts with the default max value.
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
    public List<Node> FindRange(int x, int y, float range)
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
                    if (closedSet.Contains(neighbor))
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
            n.g = 0;
        }
        return FilterArea(result);
    }

    protected virtual List<Node> FilterArea(List<Node> area)
    {
        List<Node> newNodes = new List<Node>();
        foreach (Node n in area)
        {
            if (n.unitOnNode != null)
                continue;
            newNodes.Add(n);
        }
        return newNodes;
    }


    /// <summary>
    /// Used to extend an area by a range. Can be used to find the attack range of a character based on his move area.
    /// </summary>
    /// <param name="baseArea">The area to be expanded.</param>
    /// <param name="range">The range of the expansion.</param>
    /// <param name="includeCurrentNode">If the node the character is standing should be included.</param>
    /// <returns>This function doesn't return all nodes, only the new ones.</returns>
    public List<Node> ExpandArea(List<Node> baseArea, int range, bool includeCurrentNode = false)
    {
        if (!map) // The map can't be null.
            return null;
        if (baseArea == null ? true : baseArea.Count == 0)
        {
            baseArea = new List<Node>();
            baseArea.Add(map.nodes[x, y]);
        }
        else if (includeCurrentNode)
            baseArea.Add(map.nodes[x, y]);

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
    /// Returns the total cost of the path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public float GetPathCost(List<Node> path)
    {
        if (path == null)
            return float.MaxValue;
        float cost = 0;
        foreach (Node n in path)
        {
            if (n.x == x && n.y == y)
                continue;
            cost += n.cost;
        }
        return cost;
    }

    /// <summary>
    /// Returns the node that has the shortest path to it.
    /// </summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    public Node ClosetNode(List<Node> nodes)
    {
        float cost = float.MaxValue;
        Node closest = null;
        foreach (Node n in nodes)
        {
            float temp = GetPathCost(PathFind(n));
            if (temp < cost)
            {
                cost = temp;
                closest = n;
            }
        }

        return closest;
    }

    #endregion
    #region Area Range Renderer
    /// <summary>
    /// Displays the walk range of the character using the AreaRangeRenderer script.
    /// </summary>
    public void ShowWalkRange()
    {
        if (walkRangeRenderer == null)
            return;
        List<Vector3> posList = new List<Vector3>();
        foreach (Node n in FindRange(x, y, currentStamina))
        {
            if (n.x == x && n.y == y)
                continue;
            posList.Add(new Vector3(n.x + map.nodeOffsetX, n.y + map.nodeOffsetY, 0));
        }
        walkRangeRenderer.RenderSquaresArea(posList, 0.85f);
    }

    /// <summary>
    /// Removes the walk range render.
    /// </summary>
    public void ClearWalkRange()
    {
        if (walkRangeRenderer)
            walkRangeRenderer.Clear();
    }

    /// <summary>
    /// Displays the attack range of the character using the AreaRangeRenderer script.
    /// </summary>
    public void ShowAttackRange()
    {
        if (attackRangeRenderer == null)
            return;
        List<Vector3> posList = new List<Vector3>();
        foreach (Node n in ExpandArea(FindRange(x, y, currentStamina), attackRange, true))
        {
            if (n.x == x && n.y == y)
                continue;
            posList.Add(new Vector3(n.x + map.nodeOffsetX, n.y + map.nodeOffsetY, 0));
        }
        attackRangeRenderer.RenderSquaresArea(posList, 0.85f);
    }
    /// <summary>
    /// Removes the attack range render.
    /// </summary>
    public void ClearAttackRange()
    {
        if (attackRangeRenderer)
            attackRangeRenderer.Clear();
    }
    #endregion


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
