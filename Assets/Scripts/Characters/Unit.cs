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
}
