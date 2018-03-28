using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic Enemy AI. Will attack (or move to attack) the closest target.
/// </summary>
public class EnemyAI : AIController
{
    public override void StartTurn()
    {
        base.StartTurn();
    }
    protected override Character ChooseTarget()
    {
        List<Character> targets = battleController.activeUnits;
        float dist = float.PositiveInfinity;
        Character target = null;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].team == character.team)
                continue;
            if (character.InRange(targets[i].x, targets[i].y))
            {
                target = targets[i];
                break;
            }
            List<Node> tempPath = character.PathFind(character.ClosestNode(character.map.GetNeighbors(targets[i].x, targets[i].y)));

            float temp = 0;
            if (tempPath == null ? true : tempPath.Count == 0)
                continue;
            else
                temp = character.GetPathCost(tempPath);

            if (temp < dist)
            {
                dist = temp;
                target = targets[i];
            }

        }

        return target;
    }

    protected override IEnumerator Movement()
    {

        Node movementNode = null;
        if (target != null)
        {
            if (character.InRange(target.x, target.y))
            {
                // No need to move

            }
            else if (attackArea.Contains(character.map.nodes[target.x, target.y]))
            {
                movementNode = character.ClosestNode(Map.GetClosestNode(walkArea, character.map.nodes[target.x, target.y], character.attackRange));

            }
            else
            {

                List<Node> path = character.PathFind(character.ClosestNode(character.map.GetNeighbors(target.x, target.y)));

                if (path != null ? path.Count > 0 : false)
                {
                    float totalCost = 0;
                    foreach (Node n in path)
                    {
                        totalCost += n.cost;
                        if (totalCost > character.currentStamina)
                            break;

                        movementNode = n;

                    }

                }

            }
        }
        if (movementNode != null)
        {
            yield return new WaitForSeconds(0.5f);
            character.WalkPath(character.PathFind(movementNode));
            CameraControl.instance.StartFollow(character.transform);
            while (character.IsMoving())
                yield return null;
            yield return new WaitForSeconds(CameraControl.instance.followSmoothTime);
            CameraControl.instance.StopFollow();
        }

    }

    protected override IEnumerator Action()
    {
        if (target != null ? character.InRange(target.x, target.y) : false)
        {
            yield return Attack();
        }
    }

    protected virtual IEnumerator Attack()
    {
        if (target == null)
            yield break;
        yield return character.AttackTarget(target);
        if (target.IsDown())
        {
            battleController.RemoveKnockedDown(target);
            target = null;
        }
        else if (target.CanCounter(character))
        {
            yield return target.AttackTarget(character);
        }
    }
}
