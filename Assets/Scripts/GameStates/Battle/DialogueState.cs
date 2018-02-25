using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueState : BattleState
{
    ConversationData data;

    protected override void Awake()
    {
        base.Awake();
        //TEST
        data = Resources.Load<ConversationData>("Conversations/TestConversation");
    }

    public override void Enter()
    {
        base.Enter();
        if (DialogueManager.instance)
            DialogueManager.instance.StartConversation(data);
        if (cameraControl)
            cameraControl.canMove = false;
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
        if (DialogueManager.instance)
            DialogueManager.instance.HideDialogueBox();
        if (cameraControl)
            cameraControl.canMove = true;
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
