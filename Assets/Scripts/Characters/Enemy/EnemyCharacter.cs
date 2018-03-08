using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public Character enemy;
    bool isDone;
    protected BattleController battleController;
    protected Unit target;
    protected void Start()
    {
        if (battleController == null)
            battleController = FindObjectOfType<BattleController>();
        if (enemy == null)
            enemy = GetComponent<Character>();
    }
    public virtual void ExecuteTurn()
    {
        if (enemy == null)
        {
            isDone = true;
            return;
        }
        StartCoroutine(Turn());
    }

    public virtual IEnumerator Turn()
    {
        target = ChooseTarget();
        yield return Movement();
        EndTurn();
    }

    public virtual Unit ChooseTarget()
    {
        return null;
    }

    public virtual IEnumerator Movement()
    {
        yield return null;
    }

    public void StartTurn()
    {
        isDone = false;
    }

    public virtual void EndTurn()
    {
        isDone = true;
    }

    public bool IsDone()
    {
        return isDone;
    }

}
