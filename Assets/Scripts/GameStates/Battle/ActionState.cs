using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionState : BattleState
{
    List<Node> movementNodes;
    List<Node> actionNodes;
    public override void Enter()
    {
        base.Enter();
        SetUpCharacterRange();
        StartCoroutine(CheckStatus());
    }



    public override void Exit()
    {
        base.Exit();
        ClearCharacterRange();
        ClearUI();
    }


    protected override void AddListeners()
    {
        base.AddListeners();
        if (InputManager.instance != null)
            InputManager.instance.OnBackPress += OnBackPress;
        if (CombatUIController.instance != null)
        {
            CombatUIController.instance.OnEndTurn += OnEndTurn;
            CombatUIController.instance.OnCancelMove += OnCancelMove;
        }
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        if (InputManager.instance != null)
            InputManager.instance.OnBackPress -= OnBackPress;
        if (CombatUIController.instance != null)
        {
            CombatUIController.instance.OnEndTurn -= OnEndTurn;
            CombatUIController.instance.OnCancelMove -= OnCancelMove;
        }
    }

    void SetUpCharacterRange()
    {
        movementNodes = turn.actor.FindRange(turn.actor.x, turn.actor.y, turn.actor.currentStamina);
        actionNodes = turn.actor.ExpandArea(turn.actor.FindRange(turn.actor.x, turn.actor.y, turn.actor.currentStamina), turn.actor.attackRange, true);
        if (!turn.hasUnitActed)
            turn.actor.ShowAttackRange();
        turn.actor.ShowWalkRange();
    }
    void ClearCharacterRange()
    {
        turn.actor.ClearAttackRange();
        turn.actor.ClearWalkRange();
        movementNodes = null;
        actionNodes = null;
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
                    /// TO DO
                    /// Check if target is valid if on action range.
                    /// Create method that checks the current situation.
                    /// Create interactions for different targets.
                    /// Create attack state.
                    /// Create way to confirm attack, and attack graphics.
                    /// Fix Find Range filtering characters node.
                    if (movementNodes.Contains(originNode))// The node was in the movement range
                    {
                        owner.ChangeState<MoveSequenceState>();
                    }
                    else if (actionNodes.Contains(originNode) && !turn.hasUnitActed)
                    {
                        // Action
                        if (originNode.unitOnNode == null) // No unit on node
                        {
                            if (!turn.actor.InRange(originNode))
                            {
                                SelectNode(turn.actor.ClosetNode(Map.GetClosestNode(movementNodes, originNode, turn.actor.attackRange)));
                                owner.ChangeState<MoveSequenceState>();
                            }

                        }
                        else // Unit on node
                        {
                            if (originNode.unitOnNode == turn.actor)
                                return;
                            if (!turn.actor.InRange(originNode)) // Move to attack
                            {

                                SelectNode(turn.actor.ClosetNode(Map.GetClosestNode(movementNodes, originNode, turn.actor.attackRange)));
                                owner.ChangeState<MoveSequenceState>();
                            }
                            else
                            {

                                turn.target = (Character)originNode.unitOnNode;
                                owner.ChangeState<AttackState>();
                            }
                        }
                    }
                    else
                    {
                        owner.ChangeState<ExploreState>(); // The node wasn't in the movement or action range. Go to explore mode.
                    }

                }
            }
        }
    }

    IEnumerator CheckStatus()
    {
        yield return null;
        if (CombatUIController.instance.endTurn != null)
            CombatUIController.instance.endTurn.SetActive(true);
        if (turn.hasUnitMoved)
        {
            if (CombatUIController.instance != null)
            {
                if (CombatUIController.instance.cancelMove != null)
                    CombatUIController.instance.cancelMove.SetActive(true);
            }
        }
        else
        {
            if (CombatUIController.instance != null)
            {
                if (CombatUIController.instance.cancelMove != null)
                    CombatUIController.instance.cancelMove.SetActive(false);
            }
        }
    }

    public void OnEndTurn()
    {
        owner.ChangeState<SelectTargetState>();
    }

    public void ClearUI()
    {
        if (CombatUIController.instance != null)
        {
            if (CombatUIController.instance.cancelMove != null)
                CombatUIController.instance.cancelMove.SetActive(false);
            if (CombatUIController.instance.endTurn != null)
                CombatUIController.instance.endTurn.SetActive(false);
        }
    }

    public void OnBackPress()
    {

        OnCancelMove();
    }
    public void OnCancelMove()
    {
        StartCoroutine(CancelMove());
    }

    IEnumerator CancelMove()
    {
        yield return null;
        if (turn.hasUnitMoved)
        {

            turn.UndoMove();
            SetUpCharacterRange();
            DeactivateSelectNode();
            if (CombatUIController.instance != null)
            {
                if (CombatUIController.instance.cancelMove != null)
                    CombatUIController.instance.cancelMove.SetActive(false);
            }
        }
    }
}
