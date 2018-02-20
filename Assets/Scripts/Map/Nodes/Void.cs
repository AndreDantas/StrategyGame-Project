using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Void : Node
{

    public Void(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.name = "Unknown";
        this.walkable = false;
        this.cost = 99;
    }
}
