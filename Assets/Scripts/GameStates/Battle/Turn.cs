using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Turn
{
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