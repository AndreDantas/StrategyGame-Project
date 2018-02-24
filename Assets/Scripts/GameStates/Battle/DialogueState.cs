using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueState : BattleState
{
    ConversationData data;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Enter()
    {
        base.Enter();
        if (DialogueManager.instance)
            DialogueManager.instance.StartConversation(data);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (data)
            Resources.UnloadAsset(data);
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        if (DialogueManager.instance)
            DialogueManager.instance.OnComplete += OnCompleteConversation;
    }
    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        if (DialogueManager.instance)
            DialogueManager.instance.OnComplete -= OnCompleteConversation;
    }

    void OnCompleteConversation()
    {
        owner.ChangeState<SelectTargetState>();
    }
    protected override void OnClick(Vector2 originPos, Vector2 releasePos)
    {
        if (DialogueManager.instance)
            DialogueManager.instance.NextSentence();
        else
            OnCompleteConversation();
    }
}
