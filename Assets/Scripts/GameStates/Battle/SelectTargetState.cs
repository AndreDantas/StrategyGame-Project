using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectTargetState : BattleState
{

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
                        if (originNode.unitOnNode is Character) // The click was on a character
                        {
                            currentCharacter = (Character)originNode.unitOnNode;
                            owner.ChangeState<MoveTargetState>();
                        }

                        // Handle others interactions.
                    }
                }

            }

        }
    }
}
