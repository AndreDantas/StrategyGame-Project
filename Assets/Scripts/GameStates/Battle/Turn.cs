using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Turn
{
    public static int MAX_TURN = 99;
    int _turnCount;
    public int turnCount
    {
        get
        {
            return _turnCount;
        }

        set
        {
            _turnCount = value;
            _turnCount = Mathf.Clamp(_turnCount, 0, MAX_TURN);
        }
    }
    public int turnIndex;
    public Character actor;
    public Character target;
    public bool hasUnitMoved;
    public bool hasUnitActed;
    int startX, startY;
    public void Change(Character current)
    {
        actor = current;
        target = null;
        hasUnitMoved = false;
        hasUnitActed = false;
        startX = actor.x;
        startY = actor.y;
        actor.StartTurn();
    }
    public void UndoMove()
    {
        hasUnitMoved = false;
        actor.Place(startX, startY);
        actor.currentStamina = actor.maxStamina;
    }
}