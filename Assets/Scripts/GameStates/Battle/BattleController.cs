﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : StateMachine
{

    /// <summary>
    /// The sprite of the current selected node.
    /// </summary>
    public Transform nodeSelectSprite;
    public LevelManager levelManager;
    public Map map;
    public Node selectedNode;
    public Node movementNode;
    public LevelDetails levelDetails;
    public CameraControl cameraControl;
    public float cameraMoveTime = 0.3f;
    public List<Character> activeUnits = new List<Character>();
    public List<Character> knockedDownUnits = new List<Character>();
    [ReadOnly]
    public Turn turn = new Turn();
    public GameObject turnActorIndicator;
    public GameObject turnTargetIndicator;
    public Vector2 indicatorOffset;
    public FieldInfoController fieldInfoController;

    private void Start()
    {
        ChangeState<InitBattleState>();
        levelDetails = GetComponent<LevelDetails>();
        if (levelDetails == null)
            levelDetails = gameObject.AddComponent<DefaultLevelDetails>();
        levelManager = LevelManager.instance;
        levelDetails.battleController = this;

    }
    private void Update()
    {
        if (selectedNode != null)
        {
            float posY = Camera.main.WorldToScreenPoint(new Vector2(selectedNode.x, selectedNode.y)).y;
            if (posY > Screen.height * 0.65f)
            {
                if (fieldInfoController)
                    fieldInfoController.anchoredPos = AnchoredPosition.BottomLeft;
            }
            else if (posY < Screen.height * 0.3f)
            {
                if (fieldInfoController)
                    fieldInfoController.anchoredPos = AnchoredPosition.TopLeft;
            }
        }
    }

    public void RemoveKnockedDown(Character down)
    {
        if (down.IsDown() && activeUnits != null ? activeUnits.Contains(down) : false)
        {
            if (activeUnits.IndexOf(down) <= turn.turnIndex)
                turn.turnIndex--;
            down.RemoveFromMap();
            activeUnits.Remove(down);
            knockedDownUnits.Add(down);

            down.gameObject.SetActive(false);
        }
    }
}
