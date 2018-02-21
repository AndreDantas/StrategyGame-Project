using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DestroyableNode : Node
{
    bool destroyed;
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
                _currentHp = maxHp;
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
    public DestroyableNode()
    {
        this.cost = 1;
        this.walkable = false;


    }
    public DestroyableNode(int x, int y)
    {
        this.x = x;
        this.y = y;

    }
    public DestroyableNode(DestroyableNode other)
    {
        this.name = other.name;
        this.cost = other.cost;
        this.walkable = other.walkable;
        this.x = other.x;
        this.y = other.y;
        this.parent = other.parent;
        this.team = other.team;
        this.unitOnNode = other.unitOnNode;
        this.maxHp = other.maxHp;
        this.currentHp = this.maxHp;
        this.destroyed = other.destroyed;
    }

    public void Damage(int damage)
    {
        this.currentHp -= damage;
        if (currentHp < 0)
        {
            currentHp = 0;
            destroyed = true;
        }
    }

    public bool IsDestroyed()
    {
        return destroyed;
    }
}
