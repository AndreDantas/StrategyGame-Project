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
    public Node selectedNode;
    public Node movementNode;
    public CameraControl cameraControl;
    public List<Character> units = new List<Character>();
    public Turn turn = new Turn();

    private void Start()
    {
        ChangeState<InitBattleState>();
    }
}
