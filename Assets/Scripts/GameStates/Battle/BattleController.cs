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
    public List<Character> activeUnits = new List<Character>();
    public List<Character> knockedDownUnits = new List<Character>();
    public Turn turn = new Turn();
    public GameObject turnActorIndicator;
    public GameObject turnTargetIndicator;
    public Vector2 indicatorOffset;
    public FieldInfoController fieldInfoController;

    private void Start()
    {
        ChangeState<InitBattleState>();
    }
    private void Update()
    {
        if (selectedNode != null)
        {
            float posY = Camera.main.WorldToScreenPoint(new Vector2(selectedNode.x, selectedNode.y)).y;
            if (posY > Screen.height * 0.65f)
            {
                if (fieldInfoController)
                    fieldInfoController.anchoredPos = AnchoredPosition.BottomLeft;
            }
            else if (posY < Screen.height * 0.3f)
            {
                if (fieldInfoController)
                    fieldInfoController.anchoredPos = AnchoredPosition.TopLeft;
            }
        }
    }
}
