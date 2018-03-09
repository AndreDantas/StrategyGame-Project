using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            float temp = character.GetPathCost(character.PathFind(character.map.nodes[targets[i].x, targets[i].y])); // Bug, destination cant be a character
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

            if (attackArea.Contains(character.map.nodes[target.x, target.y]))
            {
                movementNode = character.ClosestNode(Map.GetClosestNode(walkArea, character.map.nodes[target.x, target.y], character.attackRange));
            }
            else
            {
                List<Node> path = character.PathFind(character.map.nodes[target.x, target.y]);
                float totalCost = 0;
                foreach (Node n in path)
                {
                    totalCost += n.cost;
                    if (totalCost > character.currentStamina)
                    {
                        movementNode = n;
                        break;
                    }
                }
            }
        }
        if (movementNode != null)
        {
            character.WalkPath(character.PathFind(movementNode));
            while (character.IsMoving())
                yield return null;
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
