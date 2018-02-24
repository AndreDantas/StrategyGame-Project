using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveTargetState : BattleState
{
    List<Node> movementNodes;

    public override void Enter()
    {
        base.Enter();
        movementNodes = currentCharacter.FindRange(currentCharacter.x, currentCharacter.y, currentCharacter.currentStamina);
        currentCharacter.ShowAttackRange();
        currentCharacter.ShowWalkRange();
    }

    public override void Exit()
    {
        base.Exit();
        currentCharacter.ClearAttackRange();
        currentCharacter.ClearWalkRange();
        movementNodes = null;
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
                    if (movementNodes.Contains(originNode)) // The node was in the movement range
                    {
                        owner.ChangeState<MoveSequenceState>();
                    }
                    else // The node wasn't in the movement range. Return to select state.
                    {
                        owner.ChangeState<SelectTargetState>();
                    }
                }
            }
        }
    }
}
