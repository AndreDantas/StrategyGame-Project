using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This script uses the Tilegrid 2D map of tiles to update the 2D node map with the correct nodes based on reference.
/// <para>This script requires a reference to a Map script and a Tilegrid script. They both need to have the same dimensions.</para>
/// </summary>
public class MapController : MonoBehaviour
{

    public static MapController instance;
    public Map currentMap;

    public TileGrid grid;

    //TEST 
    public TextMeshProUGUI selectedNodeText;
    //
    /// <summary>
    /// The reference .txt of nodes.
    /// </summary>
    public TextAsset referenceText;
    public bool useReferenceText = false;
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
        InitiateMap();

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

    //Provisory internal map creation function. Populates the internal map by looking at the sprites name and assigning the correct node.
    public void InitiateMap()
    {
        if (grid == null || currentMap == null) // Tile grid and map can't be null.
            return;
        if (grid.sizeX != currentMap.columns || grid.sizeY != currentMap.rows) // The dimensions of the tile grid and the map have to be equal.
            return;


        if (currentMap.GenerateBaseMap()) //If the creation of the base map succeeds.
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
                        n = new Node(i, j, "Unknown", 99, false);
                    n.x = i;
                    n.y = j;

                    currentMap.nodes[i, j] = n;
                }
            }
        }
    }


}
