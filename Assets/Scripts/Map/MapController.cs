using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Xml;
using System.IO;

/// <summary>
/// This script uses the Tilegrid 2D map of tiles to update the 2D node map with the correct nodes based on reference.
/// <para>It requires a reference to a Map script and a Tilegrid script. They both need to have the same dimensions.</para>
/// <para>Also the name of the tiles used have to be the same name of the reference. Ex: Sea.tile needs a node with the same name.</para>
/// </summary>
public class MapController : MonoBehaviour
{

    public static MapController instance;
    /// <summary>
    /// The current map. 
    /// </summary>
    public Map currentMap;

    /// <summary>
    /// The tile grid.
    /// </summary>
    public TileGrid grid;

    //TEST 
    public TextMeshProUGUI selectedNodeText;
    //

    /// <summary>
    /// If it should use the reference XML file.
    /// </summary>
    public bool useReferenceText = false;
    /// <summary>
    /// The reference text file of nodes. (XML Format)
    /// </summary>
    [ConditionalHide("useReferenceText", false)]
    public TextAsset referenceText;

    /// <summary>
    /// Reference list of all normal nodes. 
    /// </summary>
    public List<Node> normalNodes;
    /// <summary>
    /// Reference list of all destroyable nodes
    /// </summary>
    public List<DestroyableNode> destroyableNodes;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (referenceText != null && useReferenceText)
        {
            LoadNodeReference();
        }
        InitializeMap();

    }

    private void Update()
    {
        //TEST
        if (currentMap && selectedNodeText != null)
        {
            Node n = currentMap.GetSelectedNode();
            if (n != null)
            {
                if (n.unitOnNode == null)
                    selectedNodeText.text = n.name;
                else
                    selectedNodeText.text = "Unit";

            }
            else
            {
                selectedNodeText.text = "";
            }
        }
        //
    }
    /// <summary>
    /// Loads the XML file with all nodes references.
    /// </summary>
    protected void LoadNodeReference()
    {
        List<Node> normalNodes = new List<Node>();
        List<DestroyableNode> destroyableNodes = new List<DestroyableNode>();

        foreach (Dictionary<string, string> nodeDict in GetXmlData(referenceText, "/Nodes/normalNodes/node"))
        {

            if (nodeDict["name"] != null ? nodeDict["name"].Trim() != "" : false)
            {
                string nodeName = "";
                float cost = 1;
                bool walkable = false;
                nodeName = nodeDict["name"];

                if (nodeDict["cost"] != null)
                    float.TryParse(nodeDict["cost"], out cost);

                if (nodeDict["walkable"] != null)
                    bool.TryParse(nodeDict["walkable"], out walkable);

                Node n = new Node();
                n.name = nodeName;
                n.cost = cost;
                n.walkable = walkable;
                normalNodes.Add(n);
            }
        }

        foreach (Dictionary<string, string> nodeDict in GetXmlData(referenceText, "/Nodes/destroyableNodes/node"))
        {

            if (nodeDict["name"] != null ? nodeDict["name"].Trim() != "" : false)
            {
                string nodeName = "";
                float cost = 1;
                bool walkable = false;
                int maxHp = 1;
                nodeName = nodeDict["name"];

                if (nodeDict["cost"] != null)
                    float.TryParse(nodeDict["cost"], out cost);

                if (nodeDict["walkable"] != null)
                    bool.TryParse(nodeDict["walkable"], out walkable);
                if (nodeDict["maxHp"] != null)
                    int.TryParse(nodeDict["maxHp"], out maxHp);

                DestroyableNode n = new DestroyableNode();
                n.name = nodeName;
                n.cost = cost;
                n.walkable = walkable;
                n.maxHp = maxHp;
                destroyableNodes.Add(n);
            }
        }

        if (destroyableNodes.Count > 1)
            this.destroyableNodes = destroyableNodes;
        if (normalNodes.Count > 1)
            this.normalNodes = normalNodes;

    }
    /// <summary>
    /// Returns a list of dictionaries containing "ParameterName, value" pairs (string, string) from a XML file.  
    /// </summary>
    /// <param name="xmlData">The TextAsset containing the XML.</param>
    /// <param name="path">The path to a specific XML node.</param>
    public static List<Dictionary<string, string>> GetXmlData(TextAsset xmlData, string path)
    {
        if (xmlData == null)
        {
            return null;
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(xmlData.text));
        XmlNodeList nodeList = xmlDoc.SelectNodes(path);

        if (nodeList != null)
        {
            List<Dictionary<string, string>> nodesRef = new List<Dictionary<string, string>>();
            foreach (XmlNode n in nodeList)
            {

                Dictionary<string, string> nodeDict = new Dictionary<string, string>();
                foreach (XmlNode param in n.ChildNodes)
                {
                    if (!nodeDict.ContainsKey(param.Name))
                        nodeDict.Add(param.Name, param.InnerText);
                }
                nodesRef.Add(nodeDict);

            }
            return nodesRef;
        }

        return null;

    }

    /// <summary>
    /// Populates the internal map by looking at the sprites name and assigning the correct node.
    /// </summary>
    public void InitializeMap()
    {
        if (grid == null || currentMap == null) // Tile grid and map can't be null.
            return;
        if (grid.sizeX != currentMap.columns || grid.sizeY != currentMap.rows) // The dimensions of the tile grid and the map have to be equal.
            return;


        if (currentMap.GenerateBaseMap()) //If the creation of the base map and the tile grid succeeds.
        {
            grid.CreateTileMap();
            for (int i = 0; i < currentMap.nodes.GetLength(0); i++)
            {
                for (int j = 0; j < currentMap.nodes.GetLength(1); j++)
                {
                    if (!grid.ValidCoordinate(i, j))
                        continue;
                    Node n = null;
                    for (int k = 0; k < normalNodes.Count; k++)
                    {
                        if (grid.tiles[i, j].tile.name.ToLower().Contains(normalNodes[k].name.ToLower())) // Check if sprite's name exist in reference
                        {
                            n = new Node(normalNodes[k]);
                            break;
                        }
                    }
                    if (n == null)
                    {
                        for (int k = 0; k < destroyableNodes.Count; k++)
                        {
                            if (grid.tiles[i, j].tile.name.ToLower().Contains(destroyableNodes[k].name.ToLower())) // Check if sprite's name exist in reference
                            {
                                n = new DestroyableNode(destroyableNodes[k]);
                                break;
                            }
                        }
                    }
                    if (n == null) // Sprite doesn't exist in reference. Creates default node.
                        n = new Node(i, j, "Unknown", 1, false);
                    n.x = i;
                    n.y = j;

                    currentMap.nodes[i, j] = n;
                }
            }
        }
    }


}
