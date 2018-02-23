using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveTargetState : BattleState
{
    //TEST
    Character selectedCharacter;

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
                    ///CHARACTER INTERACTIONS - IN TEST PROCESS///
                    if (originNode.unitOnNode == null)
                    {

                        if (selectedCharacter != null)
                        {
                            selectedCharacter.WalkPath(selectedCharacter.PathFind(originNode));
                            selectedCharacter.ClearAttackRange();
                            selectedCharacter.ClearWalkRange();
                            selectedCharacter = null;

                        }
                    }
                    else
                    {
                        if (originNode.unitOnNode is Character)
                        {
                            Character chr = (Character)originNode.unitOnNode;

                            if (selectedCharacter != null)
                            {
                                selectedCharacter.ClearAttackRange();
                                selectedCharacter.ClearWalkRange();
                            }

                            if (!chr.IsMoving())
                            {
                                selectedCharacter = chr;
                                selectedCharacter.ShowWalkRange();
                                selectedCharacter.ShowAttackRange();
                            }
                            else
                                selectedCharacter = null;
                        }
                        else
                            selectedCharacter = null;
                    }
                    //////////////////////////////////////////////
                }
                else
                {
                    //selectedNode = null;
                }
            }

        }
    }
}
