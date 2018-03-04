using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    /// <summary>
    /// Unit's map coordinates.
    /// </summary>
    public int x, y;
    /// <summary>
    /// The current map.
    /// </summary>
    public Map map;
    public int team = 0;

    /// <summary>
    /// Initializes the unit on the map.
    /// </summary>
    public virtual void InitializeOnMap()
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
    /// Removes the unit from the map.
    /// </summary>
    public virtual void RemoveFromMap()
    {
        if (map != null ? map.ValidCoordinate(x, y) ? map.nodes[x, y].unitOnNode == this : false : false)
            map.nodes[x, y].unitOnNode = null;

    }

    /// <summary>
    /// Places the unit on a point in the map.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public virtual void Place(int x, int y)
    {
        if (map == null)
            return;
        if (map.ValidCoordinate(x, y) ? map.nodes[x, y].unitOnNode == null && map.nodes[x, y].walkable == true : false)
        {
            if (map.ValidCoordinate(this.x, this.y) ? map.nodes[this.x, this.y].unitOnNode == this : false)
            {
                map.nodes[this.x, this.y].unitOnNode = null;
            }
            this.x = x;
            this.y = y;
            map.nodes[x, y].unitOnNode = this;
            transform.position = new Vector2(x + map.nodeOffsetX, y + map.nodeOffsetY);
        }
    }
}
