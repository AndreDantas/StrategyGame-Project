using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MapNavigation
{
    /// <summary>
    /// Vertical and horizontal movement.
    /// </summary>
    Cross,
    /// <summary>
    /// 8 directions movement.
    /// </summary>
    Diagonals
}
public enum NodeDistance
{
    Manhattan,
    Euclidean
}
public class Map : MonoBehaviour
{
    public MapNavigation mapNavigation = MapNavigation.Cross;
    public int rows = 10;
    public int columns = 10;
    public float nodeOffsetX = 0.5f;
    public float nodeOffsetY = 0.5f;
    public Node[,] nodes;
    public GameObject nodeSelectSprite;
    protected Node selectedNode;
    protected Character selectedCharacter;
    private void Awake()
    {
        if (ScreenClicks.instance)
        {
            ScreenClicks.instance.OnClick += OnClick;
        }


    }
    private void OnValidate()
    {
        rows = MathOperations.ClampMin(rows, 1);
        columns = MathOperations.ClampMin(columns, 1);
        transform.position = Vector3.zero;
    }
    private void Update()
    {
        PlaceSelectedNodeSprite();
    }

    /// <summary>
    /// Creates the base map with default nodes.
    /// </summary>
    /// <returns></returns>
    public bool GenerateBaseMap()
    {
        if (columns <= 0 || rows <= 0)
            return false;
        nodes = new Node[columns, rows];
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Node node = new Node(i, j);
                nodes[i, j] = node;

            }
        }
        return true;
    }

    /// <summary>
    /// Used to place the graphic on the selected Node.
    /// </summary>
    public void PlaceSelectedNodeSprite()
    {
        if (selectedNode != null ? ValidCoordinate(selectedNode) : false)
        {
            if (nodeSelectSprite != null)
            {

                nodeSelectSprite.transform.position = new Vector2(selectedNode.x + nodeOffsetX, selectedNode.y + nodeOffsetY);
                nodeSelectSprite.SetActive(true);
            }
        }
        else
        {
            if (nodeSelectSprite != null)
            {
                nodeSelectSprite.SetActive(false);
            }
        }
    }

    /// <summary>
    /// When a click is made.
    /// </summary>
    public void OnClick()
    {
        Vector2 originPos = Camera.main.ScreenToWorldPoint(ScreenClicks.instance.GetClickOriginPos());
        Vector2 releasePos = Camera.main.ScreenToWorldPoint(ScreenClicks.instance.GetClickReleasePos());

        if (ValidCoordinate(originPos) && ValidCoordinate(releasePos))
        {
            Node originNode = GetNodeFromWorldPosition(originPos);
            Node releaseNode = GetNodeFromWorldPosition(releasePos);

            if (!EventSystem.current.IsPointerOverGameObject(-1) && !ScreenClicks.IsPointerOverUIObject()) //The click wasn't on UI element.
            {

                if (originNode == releaseNode) // The click was on the same Node.
                {
                    selectedNode = originNode;
                    ///CHARACTER INTERACTIONS - IN TEST PROCESS///
                    if (originNode.unitOnNode == null)
                    {

                        if (selectedCharacter != null)
                        {
                            selectedCharacter.WalkPath(selectedCharacter.PathFind(originNode));
                            selectedCharacter = null;
                        }
                    }
                    else
                    {
                        if (originNode.unitOnNode is Character)
                            selectedCharacter = (Character)originNode.unitOnNode;
                        else
                            selectedCharacter = null;
                    }
                    //////////////////////////////////////////////
                }
                else
                {
                    //selectedNode = null;
                }
            }

        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (nodes == null || (rows == 0 || columns == 0))
            return;
        Gizmos.color = Color.white;

        List<Node> allNodes = GetNodes();
        for (int i = 0; i < allNodes.Count; i++)
        {
            //Draw selectedNode.
            if (selectedNode != null ? selectedNode == allNodes[i] : false)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(new Vector2(allNodes[i].x + nodeOffsetX, allNodes[i].y + nodeOffsetY) + (Vector2)transform.position,
                                    Vector2.one * 0.9f);
                Gizmos.color = Color.white;
            }

            //Node drawing
            Gizmos.DrawWireCube(new Vector2(allNodes[i].x + nodeOffsetX, allNodes[i].y + nodeOffsetY) + (Vector2)transform.position, Vector2.one);

            //Draw unwalkable nodes.
            if (allNodes[i].walkable == false)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawLine(new Vector3(allNodes[i].x, allNodes[i].y, 0),
                                new Vector3(allNodes[i].x + 1, allNodes[i].y + 1, 0));
                Gizmos.DrawLine(new Vector3(allNodes[i].x, allNodes[i].y + 1, 0),
                                new Vector3(allNodes[i].x + 1, allNodes[i].y, 0));
                Gizmos.color = Color.white;
            }
        }

    }
#endif
    /// <summary>
    /// Returns a list with all the nodes.
    /// </summary>
    /// <returns></returns>
    public List<Node> GetNodes()
    {
        if (nodes == null)
            return null;
        List<Node> result = new List<Node>();
        for (int i = 0; i < nodes.GetLength(0); i++)
        {
            for (int j = 0; j < nodes.GetLength(1); j++)
            {

                result.Add(nodes[i, j]);
            }
        }
        return result;
    }

    /// <summary>
    /// Returns the selected node.
    /// </summary>
    /// <returns></returns>
    public Node GetSelectedNode()
    {
        return selectedNode;
    }

    /// <summary>
    /// Returns node's neighbors.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public List<Node> GetNeighbors(Node node)
    {
        if (ValidCoordinate(node.x, node.y))
        {
            List<Node> result = new List<Node>();
            if (mapNavigation == MapNavigation.Cross)
            {
                //Left neighbor
                if (ValidCoordinate(node.x - 1, node.y))
                {
                    result.Add(nodes[node.x - 1, node.y]);
                }
                //Right neighbor
                if (ValidCoordinate(node.x + 1, node.y))
                {
                    result.Add(nodes[node.x + 1, node.y]);
                }
                //Bottom neighbor
                if (ValidCoordinate(node.x, node.y - 1))
                {
                    result.Add(nodes[node.x, node.y - 1]);
                }
                //Top neighbor
                if (ValidCoordinate(node.x, node.y + 1))
                {
                    result.Add(nodes[node.x, node.y + 1]);
                }
            }
            else
            {
                for (int i = node.x - 1; i <= node.x + 1; i++)
                {
                    for (int j = node.y - 1; j <= node.y + 1; j++)
                    {
                        if (i == node.x && j == node.y)
                            continue;
                        else
                        {
                            if (ValidCoordinate(i, j))
                            {
                                result.Add(nodes[i, j]);
                            }
                        }
                    }
                }
            }
            return result;
        }
        else
            return null;
    }

    /// <summary>
    /// Returns node's neighbors.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public List<Node> GetNeighbors(int x, int y)
    {
        return GetNeighbors(nodes[x, y]);
    }
    /// <summary>
    /// Checks if the coordinate is valid in this map.
    /// </summary>
    /// <returns></returns>
    public bool ValidCoordinate(Vector2 worldPos)
    {

        int tilePosX = (int)Mathf.Floor(worldPos.x);
        int tilePosY = (int)Mathf.Floor(worldPos.y);

        return ValidCoordinate(tilePosX, tilePosY);
    }

    /// <summary>
    /// Checks if the coordinate is valid in this map.
    /// </summary>
    /// <returns></returns>
    public bool ValidCoordinate(int x, int y)
    {
        if (nodes == null)
            return false;
        if (x < 0 || x >= nodes.GetLength(0))
            return false;
        if (y < 0 || y >= nodes.GetLength(1))
            return false;

        return true;
    }
    /// <summary>
    /// Checks if the coordinate is valid in this map.
    /// </summary>
    /// <returns></returns>
    public bool ValidCoordinate(Node node)
    {
        return ValidCoordinate(node.x, node.y);
    }

    /// <summary>
    /// Returns the node located on that position.
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public Node GetNodeFromWorldPosition(Vector2 worldPos)
    {
        if (ValidCoordinate(worldPos))
        {
            int tilePosX = (int)Mathf.Floor(worldPos.x);
            int tilePosY = (int)Mathf.Floor(worldPos.y);
            return nodes[tilePosX, tilePosY];
        }
        return null;
    }
    /// <summary>
    /// Returns the world position of the node.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 GetWorldPositionFromNode(int x, int y)
    {
        if (ValidCoordinate(x, y))
        {
            return new Vector3(nodes[x, y].x + nodeOffsetX, nodes[x, y].y + nodeOffsetY, 0);
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Returns the world position of the node.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetWorldPositionFromNode(Node node)
    {
        return GetWorldPositionFromNode(node.x, node.y);
    }
    /// <summary>
    /// Distance between two nodes.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float Distance(Node a, Node b, NodeDistance distance = NodeDistance.Manhattan)
    {
        if (distance == NodeDistance.Manhattan)
        {
            float d = 0.5f;
            return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)) * d;
        }
        else
            return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
    }
#if UNITY_EDITOR
    public void PrintGrid()
    {
        if (nodes == null)
            return;
        string result = "";
        for (int y = nodes.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < nodes.GetLength(0); x++)
            {

                result += nodes[x, y].name[0];
                if (x != nodes.GetLength(0) - 1)
                    result += "|";
                else
                    result += "\n";
            }
        }
        print(result);
    }
#endif
}
