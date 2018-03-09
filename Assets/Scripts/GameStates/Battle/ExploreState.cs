using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ExploreState : BattleState
{

    public override void Enter()
    {
        base.Enter();
        turn.target = null;
        ShowActorIndicator();
    }
    protected override void AddListeners()
    {
        base.AddListeners();
        if (InputManager.instance != null)
            InputManager.instance.OnBackPress += OnBackPress;
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        if (InputManager.instance != null)
            InputManager.instance.OnBackPress -= OnBackPress;
    }

    protected override void OnClick(Vector2 originPos, Vector2 releasePos)
    {
        if (map == null)
            return;

        originPos = Camera.main.ScreenToWorldPoint(originPos);
        releasePos = Camera.main.ScreenToWorldPoint(releasePos);

        if (map.ValidCoordinate(originPos) && map.ValidCoordinate(releasePos))
        {
            Node originNode = map.GetNodeFromWorldPosition(originPos);
            Node releaseNode = map.GetNodeFromWorldPosition(releasePos);

            if (!EventSystem.current.IsPointerOverGameObject(-1) && !ScreenClicks.IsPointerOverUIObject()) //The click wasn't on UI element.
            {

                if (originNode == releaseNode) // The click was on the same Node.
                {
                    SelectNode(originNode);
                    UpdateFieldInfoBox(originNode);
                    if (originNode.unitOnNode != null)
                    {
                        if (originNode.unitOnNode is Character)
                        {
                            Character target = (Character)originNode.unitOnNode;
                            if (target == turn.actor)
                            {
                                owner.ChangeState<PlayerState>();
                                return;
                            }


                        }
                    }

                }
            }

        }
    }

    public void UpdateFieldInfoBox(Node n)
    {
        if (fieldInfoController != null)
        {
            fieldInfoController.ShowFieldInfoBox(n);
        }
    }

    public void OnBackPress()
    {
        owner.ChangeState<PlayerState>();
        DeactivateSelectNode();
        //Center camera on current character
        Camera.main.transform.position = new Vector3(turn.actor.transform.position.x, turn.actor.transform.position.y, Camera.main.transform.position.z);
    }
}
