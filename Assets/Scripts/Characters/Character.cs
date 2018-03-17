using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LookDirection
{
    Left,
    Right
}
public class Character : Unit
{
    #region Parameters
    /// <summary>
    /// If the character is moving.
    /// </summary>
    protected bool isMoving;
    /// <summary>
    /// If the character is attacking.
    /// </summary>
    protected bool isAttacking;
    /// <summary>
    /// If the character is taking damage.
    /// </summary>
    protected bool isTakingDamage;
    protected bool isDown;

    [SerializeField]
    LookDirection _lookDirection = LookDirection.Right;

    /// <summary>
    /// The direction the character is facing.
    /// </summary>
    public LookDirection lookDirection
    {
        get
        {
            return _lookDirection;
        }

        set
        {
            _lookDirection = value;
            switch (_lookDirection)
            {
                case LookDirection.Right:
                    if (sprite)
                    {
                        sprite.flipX = false;
                    }
                    break;
                case LookDirection.Left:
                    if (sprite)
                    {
                        sprite.flipX = true;
                    }
                    break;
            }
        }
    }
    public float moveTime = 0.1f;
    public HealthController healthBar;
    public SpriteRenderer sprite;
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
    [Range(0.3f, 5f)]
    public float attackTime = 0.4f;
    public AreaRangeRenderer walkRangeRenderer;
    public AreaRangeRenderer attackRangeRenderer;
    public float areaRangeSize = 0.85f;
    public HitAnimation hitAnimation;
    #endregion

    protected virtual void OnValidate()
    {
        //TEST
        transform.position = new Vector2(x + 0.5f, y + 0.5f);
        switch (lookDirection)
        {
            case LookDirection.Right:
                if (sprite)
                {
                    sprite.flipX = false;
                }
                break;
            case LookDirection.Left:
                if (sprite)
                {
                    sprite.flipX = true;
                }
                break;
        }

    }
    protected virtual void Start()
    {
        if (map == null)
            map = FindObjectOfType<Map>();
        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();
        InitializeOnMap();
        if (healthBar)
        {
            if (healthBar.GetBar() == null)
                healthBar.SetBar(GetComponentInChildren<Bar>());
            healthBar.MaxValue = maxHp;
            healthBar.CurrentValue = currentHp;
        }
        healthUI = GetComponentInChildren<Canvas>();
        ShowHealthBar();
    }

    /// <summary>
    /// Shows the health UI.
    /// </summary>
    public void ShowHealthBar()
    {
        if (healthUI)
        {
            healthUI.gameObject.SetActive(true);
            healthUI.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0.6f);
        }
    }
    /// <summary>
    /// Hides the health UI.
    /// </summary>
    public void HideHealthBar()
    {
        if (healthUI)
        {
            healthUI.gameObject.SetActive(false);
        }
    }

    public override void InitializeOnMap()
    {
        base.InitializeOnMap();
        isDown = false;
        currentHp = maxHp;
        currentStamina = maxStamina;
    }


    /// <summary>
    /// To be used at the start of each turn.
    /// </summary>
    public virtual void StartTurn()
    {
        currentStamina = maxStamina;
    }

    #region Attack and Damage
    /// <summary>
    /// Returns the base attack + modifiers.
    /// </summary>
    /// <returns></returns>
    public virtual int Attack()
    {
        return attack + (map.ValidCoordinate(x, y) ? map.nodes[x, y].attackBonus : 0);
    }
    /// <summary>
    /// Deals damage to character.
    /// </summary>
    /// <param name="damage"></param>
    public virtual void Damage(int damage)
    {

        this.currentHp -= DamageEvaluation(damage);
        if (currentHp <= 0)
        {
            currentHp = 0;
            isDown = true;
        }
    }

    /// <summary>
    /// Changes damage based on modifiers.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public virtual int DamageEvaluation(int damage)
    {
        return MathOperations.ClampMin(damage - (defense + (map.ValidCoordinate(x, y) ? map.nodes[x, y].defenseBonus : 0)), 0);
    }

    /// <summary>
    /// Starts the attack animation.
    /// </summary>
    /// <param name="x">The X coordinate of the target.</param>
    /// <param name="y">The Y coordinate of the target.</param>
    public virtual void AttackAnim(int x, int y)
    {
        StartCoroutine(AttackAnimation(x, y));
    }

    public IEnumerator AttackTarget(Character target)
    {
        if (target.x > x)
            lookDirection = LookDirection.Right;
        else if (target.x < x)
            lookDirection = LookDirection.Left;

        AttackAnim(target.x, target.y); // Start attack animation.

        while (IsAttacking())
            yield return null;

        yield return null;

        target.DamageAnim(Attack()); // Start damage animation.

        while (target.IsTakingDamage())
            yield return null;

        target.Damage(Attack()); // Deal damage to target.

    }


    protected virtual IEnumerator AttackAnimation(int x, int y)
    {
        isAttacking = true;
        yield return null;
        Vector2 attackDirection = (new Vector2(x, y) - new Vector2(this.x, this.y)).normalized;

        float t = 0;
        float lerpTime = 0;
        Vector2 origin = transform.position;
        Vector2 start = transform.position;
        Vector2 end = (Vector2)transform.position + ((attackDirection * -1) * 0.5f);
        while (t < 0.95f || lerpTime < attackTime)
        {
            t = lerpTime / attackTime;
            t = MathOperations.ChangeLerpT(LerpMode.EaseIn, t);
            transform.position = Vector2.Lerp(start, end, t);
            lerpTime += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
        start = end;
        end = origin + (attackDirection * 0.5f);
        t = 0;
        lerpTime = 0;
        while (t < 0.95f || lerpTime < attackTime / 4f)
        {
            t = lerpTime / (attackTime / 4f);
            t = MathOperations.ChangeLerpT(LerpMode.EaseOut, t);
            transform.position = Vector2.Lerp(start, end, t);
            lerpTime += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
        start = end;
        end = origin;
        t = 0;
        lerpTime = 0;
        while (t < 0.95f || lerpTime < attackTime / 2f)
        {
            t = lerpTime / (attackTime / 2f);
            t = MathOperations.ChangeLerpT(LerpMode.Exponential, t);
            transform.position = Vector2.Lerp(start, end, t);
            lerpTime += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
        if (hitAnimation)
        {
            hitAnimation.gameObject.transform.position = new Vector2(x + map.nodeOffsetX, y + map.nodeOffsetY);
            hitAnimation.Play();
            yield return new WaitForSeconds(hitAnimation.AnimLength() * 0.5f);
        }
        isAttacking = false;

    }

    /// <summary>
    /// Starts the damage animation.
    /// </summary>
    /// <param name="damage"></param>
    public virtual void DamageAnim(int damage)
    {
        StartCoroutine(DamageAnimation(DamageEvaluation(damage)));
    }

    protected virtual IEnumerator DamageAnimation(int damage)
    {
        if (isTakingDamage || healthBar == null)
        {
            yield break;
        }
        isTakingDamage = true;
        yield return null;
        yield return healthBar.DamageAnim(damage);

        isTakingDamage = false;
    }

    public virtual bool CanCounter(Character other)
    {
        return Map.DefaultManhattanDistance(x, y, other.x, other.y) <= attackRange;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public bool IsTakingDamage()
    {
        return isTakingDamage;
    }

    public bool IsDown()
    {
        return isDown;
    }
    #endregion
    #region Movement
    /// <summary>
    /// Moves the character between nodes.
    /// </summary>
    /// <param name="destination">The destination node.</param>
    /// <param name="duration">How long the movement takes in seconds.</param>
    /// <returns></returns>
    protected IEnumerator LerpMove(Node destination)
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

        if (destination.x > x)
            lookDirection = LookDirection.Right;
        else if (destination.x < x)
            lookDirection = LookDirection.Left;

        while (t < 1)
        {
            t = lerpTime / moveTime;
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
        if (isMoving)
            yield break;

        // TEST
        if (GetPathCost(path) > currentStamina)
            yield break;
        ClearAttackRange();
        ClearWalkRange();
        ////
        isMoving = true;
        foreach (Node n in path)
        {
            yield return LerpMove(n);
        }
        isMoving = false;
    }
    /// <summary>
    /// If the character is moving.
    /// </summary>
    /// <returns></returns>
    public bool IsMoving()
    {
        return isMoving;
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
        // Algorithm used: https://services.studiomonolith.ca/redirect/index.php?q=nNjZ1mtnYquYnaGdqqmZm52h1cZflaTSYGZpZJuQYm9jlJ2VpZmWq5mbqGGrpJdkp5nXyV6YntOVnaealQ

        if (!map) // The map can't be null.
            return null;
        if (!map.ValidCoordinate(x, y))
            return null;

        List<Node> result = new List<Node>();
        foreach (Node n in map.GetNodes())
        {
            n.g = float.PositiveInfinity;
        }

        Queue<Node> checkNext = new Queue<Node>();
        Queue<Node> checkNow = new Queue<Node>();
        Node current = map.nodes[x, y];
        current.g = 0;
        checkNow.Enqueue(current);

        while (checkNow.Count > 0)
        {
            current = checkNow.Dequeue();
            List<Node> neighbors = map.GetNeighbors(current);
            for (int i = 0; i < neighbors.Count; i++)
            {
                float newG = current.g + NodeCostEvaluation(neighbors[i]);
                if (!ValidNode(neighbors[i]) || neighbors[i].g <= newG || newG > range)
                    continue;

                neighbors[i].g = newG;
                neighbors[i].parent = current;
                checkNext.Enqueue(neighbors[i]);
                result.Add(neighbors[i]);
            }
            if (checkNow.Count == 0)
                SwapReference(ref checkNow, ref checkNext);
        }
        return FilterArea(result);
    }
    static void SwapReference(ref Queue<Node> a, ref Queue<Node> b)
    {
        Queue<Node> temp = a;
        a = b;
        b = temp;
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
    public Node ClosestNode(List<Node> nodes)
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

    public bool InRange(Node n)
    {
        return Map.DefaultManhattanDistance(n, new Node(x, y)) <= attackRange;
    }

    public bool InRange(int x, int y)
    {
        return Map.DefaultManhattanDistance(this.x, this.y, x, y) <= attackRange;
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
        List<Node> closed = new List<Node>();
        foreach (Node n in FindRange(x, y, currentStamina))
        {
            if (n.x == x && n.y == y || closed.Contains(n))
                continue;
            closed.Add(n);
            posList.Add(new Vector3(n.x + map.nodeOffsetX, n.y + map.nodeOffsetY, 0));
        }

        walkRangeRenderer.RenderSquaresArea(posList, areaRangeSize);
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
        attackRangeRenderer.RenderSquaresArea(posList, areaRangeSize);
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
        // Can be modified to find a new destination and return the new node.
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
