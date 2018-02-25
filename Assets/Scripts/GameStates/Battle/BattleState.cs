using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState : State
{

    protected BattleController owner;
    public Transform nodeSelectSprite { get { return owner.nodeSelectSprite; } }
    public Map map { get { return owner.map; } }
    public Node currentNode { get { return owner.currentNode; } set { owner.currentNode = value; } }
    public CameraControl cameraControl { get { return owner.cameraControl; } }
    public List<Character> units { get { return owner.units; } }
    public Turn turn { get { return owner.turn; } }

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
        if (node == currentNode || map != null ? !map.ValidCoordinate(node) : true)
            return;

        currentNode = map.nodes[node.x, node.y];
        if (nodeSelectSprite)
        {
            nodeSelectSprite.position = map.GetWorldPositionFromNode(currentNode);
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
        }
    }
}
