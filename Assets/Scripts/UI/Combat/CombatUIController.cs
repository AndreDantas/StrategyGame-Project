using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CombatUIController : MonoBehaviour
{

    public static CombatUIController instance;
    public GameObject cancelMove;
    public GameObject endTurn;
    public GameObject fieldInfoBox;
    public TextMeshProUGUI nodeName;
    public TextMeshProUGUI nodeCost;
    public TextMeshProUGUI nodeAtkBonus;
    public TextMeshProUGUI nodeDefBonus;

    public delegate void CombatUIEventHandler();
    public event CombatUIEventHandler OnCancelMove;
    public event CombatUIEventHandler OnEndTurn;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
            instance = this;
    }

    public void SetNodeStats(Node n)
    {
        if (n == null)
            return;
        if (nodeName)
            nodeName.text = n.name;
        if (nodeCost)
            nodeCost.text = n.cost.ToString();
        if (nodeAtkBonus)
            nodeAtkBonus.text = n.attackBonus.ToString();
        if (nodeDefBonus)
            nodeDefBonus.text = n.defenseBonus.ToString();
    }

    public void HideFieldInfoBox()
    {
        if (fieldInfoBox)
            fieldInfoBox.SetActive(false);
    }

    public void ShowFieldInfoBox(Node n = null)
    {
        if (n != null)
            SetNodeStats(n);
        if (fieldInfoBox)
            fieldInfoBox.SetActive(true);
    }
    public void CancelMove()
    {
        if (OnCancelMove != null)
            OnCancelMove();
    }

    public void EndTurn()
    {
        if (OnEndTurn != null)
            OnEndTurn();
    }
}
