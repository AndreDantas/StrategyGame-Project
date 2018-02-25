using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIController : MonoBehaviour
{

    public static CombatUIController instance;
    public GameObject cancelMove;
    public GameObject endTurn;

    public delegate void CombatUIEventHandler();
    public event CombatUIEventHandler OnCancelMove;
    public event CombatUIEventHandler OnEndTurn;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
            instance = this;
    }



    public void CancelMove()
    {
        if (OnCancelMove != null)
            OnCancelMove();
    }

    public void EndTurn()
    {
        if (OnEndTurn != null)
            OnEndTurn();
    }
}
