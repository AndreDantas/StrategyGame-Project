using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ExploreState : BattleState
{
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
                    if (originNode.unitOnNode != null)
                    {
                        if ((Character)originNode.unitOnNode == turn.actor)
                        {
                            owner.ChangeState<ActionState>();
                        }
                    }

                }
            }
        }
    }

    public void OnBackPress()
    {
        owner.ChangeState<ActionState>();

        //Center camera on current character
        Camera.main.transform.position = new Vector3(turn.actor.transform.position.x, turn.actor.transform.position.y, Camera.main.transform.position.z);
    }
}
