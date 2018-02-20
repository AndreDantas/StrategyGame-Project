using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : Node
{
    public Ground(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.name = "Ground";
        this.cost = 1;
    }

}
