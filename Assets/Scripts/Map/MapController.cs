using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapController : MonoBehaviour
{

    public static MapController instance;
    public Map currentMap;

    public TileGrid grid;
    //TEST 
    public TextMeshProUGUI selectedNodeText;
    //
    public List<Node> availableNodes;


    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        InitiateLevel();

    }

    private void Update()
    {
        //TEST
        if (currentMap && selectedNodeText != null)
        {
            Node n = currentMap.GetSelectedNode();
            if (n != null)
            {
                selectedNodeText.text = n.name;
            }
            else
            {
                selectedNodeText.text = "";
            }
        }
        //
    }

    //Provisory internal map creation function. Populates the internal map by looking at the sprites name and assigning the correct node.
    public void InitiateLevel()
    {
        if (grid == null || currentMap == null)
            return;

        grid.CreateTileMap();
        if (currentMap.GenerateBaseMap())
        {
            for (int i = 0; i < currentMap.nodes.GetLength(0); i++)
            {
                for (int j = 0; j < currentMap.nodes.GetLength(1); j++)
                {
                    if (!grid.ValidCoordinate(i, j))
                        continue;
                    Node n = null;
                    for (int k = 0; k < availableNodes.Count; k++)
                    {
                        if (grid.tiles[i, j].tile.name.ToLower().Contains(availableNodes[k].name.ToLower())) // Check if sprite's name exist in reference
                        {
                            n = new Node(availableNodes[k]);

                            break;
                        }
                    }
                    if (n == null)// Sprite doesn't exist in reference. Creates default node.
                        n = new Node(i, j, "Unknown", 99, false);
                    n.x = i;
                    n.y = j;

                    currentMap.nodes[i, j] = n;
                }
            }
        }
    }
}
