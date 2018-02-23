using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState : State
{

    protected BattleController owner;
    public Transform nodeSelectSprite { get { return owner.nodeSelectSprite; } }
    public Map map { get { return owner.map; } }
    public Node pos { get { return owner.pos; } set { owner.pos = value; } }

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
            ScreenClicks.instance.OnClick += OnClick;
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
        if (node == pos || map != null ? !map.ValidCoordinate(node) : true)
            return;

        pos = map.nodes[node.x, node.y];
        if (nodeSelectSprite)
        {
            nodeSelectSprite.position = map.GetWorldPositionFromNode(pos);
            if (!nodeSelectSprite.gameObject.activeSelf)
                nodeSelectSprite.gameObject.SetActive(true);
        }
    }
}
