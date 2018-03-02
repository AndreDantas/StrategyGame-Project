using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Battle base state.
/// </summary>
public class BattleState : State
{

    protected BattleController owner;
    public Transform nodeSelectSprite { get { return owner.nodeSelectSprite; } }
    public Map map { get { return owner.map; } }
    public Node selectedNode { get { return owner.selectedNode; } set { owner.selectedNode = value; } }
    public Node movementNode { get { return owner.movementNode; } set { owner.movementNode = value; } }
    public CameraControl cameraControl { get { return owner.cameraControl; } }
    public List<Character> activeUnits { get { return owner.activeUnits; } }
    public List<Character> knockedDownUnits { get { return owner.knockedDownUnits; } }
    public Turn turn { get { return owner.turn; } }
    public GameObject turnActorIndicator { get { return owner.turnActorIndicator; } }
    public GameObject turnTargetIndicator { get { return owner.turnTargetIndicator; } }
    public FieldInfoController fieldInfoController { get { return owner.fieldInfoController; } }

    protected virtual void Awake()
    {
        owner = GetComponent<BattleController>();
    }

    protected override void AddListeners()
    {
        //Subscribing to ScreenClicks OnClick event.
        if (ScreenClicks.instance)
        {
            ScreenClicks.instance.OnClick += OnClick;
        }
    }
    protected override void RemoveListeners()
    {
        //Unsubscribing to ScreenClicks OnClick event.
        if (ScreenClicks.instance)
        {
            ScreenClicks.instance.OnClick -= OnClick;
        }
    }
    protected virtual void OnClick(Vector2 originPos, Vector2 releasePos)
    {

    }

    /// <summary>
    /// Used to place the graphic on the selected Node.
    /// </summary>
    protected virtual void SelectNode(Node node)
    {
        if (node == selectedNode || map != null ? !map.ValidCoordinate(node) : true)
            return;

        selectedNode = map.nodes[node.x, node.y];
        if (nodeSelectSprite)
        {
            nodeSelectSprite.position = map.GetWorldPositionFromNode(selectedNode);
            if (!nodeSelectSprite.gameObject.activeSelf)
                nodeSelectSprite.gameObject.SetActive(true);
        }
    }

    protected virtual void DeactivateSelectNode()
    {
        if (nodeSelectSprite)
        {

            if (nodeSelectSprite.gameObject.activeSelf)
                nodeSelectSprite.gameObject.SetActive(false);
            selectedNode = null;
        }
    }


}
