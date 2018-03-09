using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIController : MonoBehaviour
{

    public Character character;
    protected bool isDone;
    protected BattleController battleController;
    protected Character target;
    public List<Node> walkArea;
    public List<Node> attackArea;
    protected void Awake()
    {
        if (battleController == null)
            battleController = FindObjectOfType<BattleController>();
        if (character == null)
            character = GetComponent<Character>();
    }
    /// <summary>
    /// Executes the turn. This function is called by the AIState during combat.
    /// </summary>
    public virtual void ExecuteTurn()
    {
        if (character == null)
        {
            isDone = true;
            return;
        }
        StartCoroutine(Turn());
    }

    protected virtual IEnumerator Turn()
    {
        StartTurn();
        target = ChooseTarget();
        yield return Movement();
        yield return Action();
        EndTurn();
    }

    /// <summary>
    /// The function to choose a target.
    /// </summary>
    /// <returns></returns>
    protected virtual Character ChooseTarget()
    {
        return null;
    }

    /// <summary>
    /// The function to determine the movement.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Movement()
    {
        yield return null;
    }

    /// <summary>
    /// The action after movement.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Action()
    {
        yield return null;
    }

    /// <summary>
    /// Executes at the start of the turn. It's unrelated to the character StartTurn() function.
    /// </summary>
    public virtual void StartTurn()
    {
        isDone = false;
        target = null;
        walkArea = character.FindRange(character.x, character.y, character.currentStamina);
        attackArea = character.ExpandArea(walkArea, character.attackRange, true);
    }


    /// <summary>
    /// Executes at the end of the turn.
    /// </summary>
    public virtual void EndTurn()
    {

        isDone = true;
        if (character.IsDown())
            battleController.RemoveKnockedDown(character);
    }

    /// <summary>
    /// If the AI is done.
    /// </summary>
    /// <returns></returns>
    public bool IsDone()
    {
        return isDone;
    }
}
