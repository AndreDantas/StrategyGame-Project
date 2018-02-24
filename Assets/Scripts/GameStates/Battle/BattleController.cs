using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : StateMachine
{

    /// <summary>
    /// The sprite of the current selected node.
    /// </summary>
    public Transform nodeSelectSprite;
    public Map map;
    public Node currentNode;
    public Character currentCharacter;

    private void Start()
    {
        ChangeState<InitBattleState>();
    }
}
