using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum AnchoredPosition
{
    TopLeft,
    BottomLeft,
    TopRight,
    BottomRight
}

public class FieldInfoController : MonoBehaviour
{
    public GameObject fieldInfoBox;
    [SerializeField]
    AnchoredPosition _anchoredPos;
    public AnchoredPosition anchoredPos
    {
        get
        {
            return _anchoredPos;
        }

        set
        {
            _anchoredPos = value;
            ChangeAnchoredPos(value);

        }
    }
    public float animationTime = 0.15f;
    public Vector2 offset = new Vector2(10, 10);
    public TextMeshProUGUI nodeName;
    public TextMeshProUGUI nodeCost;
    public SpriteSwap nodeWalkIcon = new SpriteSwap();
    public TextMeshProUGUI nodeAtkBonus;
    public TextMeshProUGUI nodeDefBonus;

    private void Start()
    {
        ChangeAnchoredPos(anchoredPos);

    }
    private void OnValidate()
    {
        ChangeAnchoredPos(anchoredPos);

    }

    public void SetNodeStats(Node n)
    {
        if (n == null)
            return;
        if (nodeName)
            nodeName.text = n.name;
        if (nodeCost)
            nodeCost.text = n.walkable ? n.cost.ToString() : "-";
        if (nodeAtkBonus)
            nodeAtkBonus.text = n.attackBonus.ToString();
        if (nodeDefBonus)
            nodeDefBonus.text = n.defenseBonus.ToString();
        if (n.walkable)
        {
            nodeWalkIcon.Swap(0, Color.green);
        }
        else
        {
            nodeWalkIcon.Swap(1, Color.red);
        }
    }

    public void HideFieldInfoBox()
    {
        StartCoroutine(InfoBoxHideAnim());
    }

    public void ShowFieldInfoBox(Node n = null)
    {
        if (n == null)
            return;

        SetNodeStats(n);
        StartCoroutine(InfoBoxShowAnim());
    }

    IEnumerator InfoBoxHideAnim()
    {
        yield return null;

        if (fieldInfoBox == null)
            yield break;
        if (!fieldInfoBox.activeSelf)
            yield break;

        fieldInfoBox.SetActive(true);


        RectTransform rect = fieldInfoBox.GetComponent<RectTransform>();
        Vector2 start = UpdateAnchoredPos();

        float moveY = (rect.rect.height + offset.y * 2) * -Mathf.Sign(rect.anchoredPosition.y);

        Vector2 end = new Vector2(rect.localPosition.x, rect.localPosition.y + moveY);
        rect.localPosition = start;
        float t = 0;
        float lerpTime = 0;
        while (t < 0.99f)
        {

            t = lerpTime / animationTime;
            t = MathOperations.ChangeLerpT(LerpMode.EaseIn, t);
            rect.localPosition = Vector3.Lerp(start, end, t);
            lerpTime += Time.unscaledDeltaTime;
            yield return null;
        }
        rect.localPosition = end;

        fieldInfoBox.SetActive(false);
    }

    IEnumerator InfoBoxShowAnim()
    {
        yield return null;

        if (fieldInfoBox == null)
            yield break;
        if (fieldInfoBox.activeSelf)
            yield break;

        fieldInfoBox.SetActive(true);
        ChangeAnchoredPos(anchoredPos);
        RectTransform rect = fieldInfoBox.GetComponent<RectTransform>();

        float moveY = rect.rect.height + offset.y * 2;
        //float moveX = rect.rect.width + offset.x * 2;
        Vector2 start = GetUpdatedAnchoredPos() + new Vector2(0, moveY * -Mathf.Sign(rect.anchoredPosition.y));
        rect.anchoredPosition = start;
        Vector2 end = GetUpdatedAnchoredPos();

        float t = 0;
        float lerpTime = 0;
        while (t < 0.99f)
        {
            t = lerpTime / animationTime;
            t = MathOperations.ChangeLerpT(LerpMode.EaseOut, t);
            rect.anchoredPosition = Vector3.Lerp(start, end, t);
            lerpTime += Time.unscaledDeltaTime;
            yield return null;
        }
        rect.anchoredPosition = end;

    }


    public Vector2 UpdateAnchoredPos()
    {
        RectTransform rect = fieldInfoBox.GetComponent<RectTransform>();
        switch (anchoredPos)
        {
            case AnchoredPosition.TopLeft:
                rect.anchoredPosition = new Vector3(rect.rect.width * rect.pivot.x + offset.x, -(rect.rect.height * rect.pivot.y + offset.y), rect.localPosition.z);
                break;
            case AnchoredPosition.BottomLeft:
                rect.anchoredPosition = new Vector3(rect.rect.width * rect.pivot.x + offset.x, rect.rect.height * rect.pivot.y + offset.y, rect.localPosition.z);
                break;
            case AnchoredPosition.TopRight:
                rect.anchoredPosition = new Vector3(-(rect.rect.width * rect.pivot.x + offset.x), -(rect.rect.height * rect.pivot.y + offset.y), rect.localPosition.z);
                break;
            case AnchoredPosition.BottomRight:
                rect.anchoredPosition = new Vector3(-(rect.rect.width * rect.pivot.x + offset.x), rect.rect.height * rect.pivot.y + offset.y, rect.localPosition.z);
                break;

        }
        return rect.localPosition;
    }

    public Vector2 GetUpdatedAnchoredPos()
    {
        RectTransform rect = fieldInfoBox.GetComponent<RectTransform>();
        Vector2 pos = Vector2.zero;
        switch (anchoredPos)
        {
            case AnchoredPosition.TopLeft:
                pos = new Vector3(rect.rect.width / 2 + offset.x, -(rect.rect.height / 2 + offset.y), rect.localPosition.z);
                break;
            case AnchoredPosition.BottomLeft:
                pos = new Vector3(rect.rect.width / 2 + offset.x, rect.rect.height / 2 + offset.y, rect.localPosition.z);
                break;
            case AnchoredPosition.TopRight:
                pos = new Vector3(-(rect.rect.width / 2 + offset.x), -(rect.rect.height / 2 + offset.y), rect.localPosition.z);
                break;
            case AnchoredPosition.BottomRight:
                pos = new Vector3(-(rect.rect.width / 2 + offset.x), rect.rect.height / 2 + offset.y, rect.localPosition.z);
                break;

        }
        return pos;
    }

    public void ChangeAnchoredPos(AnchoredPosition pos)
    {

        Vector4 newPos = GetAnchoredPosition(pos);
        if (fieldInfoBox != null)
        {
            RectTransform rect = fieldInfoBox.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(newPos.x, newPos.y);
            rect.anchorMax = new Vector2(newPos.z, newPos.w);
            UpdateAnchoredPos();
        }
    }

    static Vector4 GetAnchoredPosition(AnchoredPosition pos = AnchoredPosition.TopLeft)
    {
        switch (pos)
        {
            case AnchoredPosition.TopLeft:
                return new Vector4(0, 1, 0, 1);

            case AnchoredPosition.BottomLeft:
                return new Vector4(0, 0, 0, 0);

            case AnchoredPosition.TopRight:
                return new Vector4(1, 1, 1, 1);

            case AnchoredPosition.BottomRight:
                return new Vector4(1, 0, 1, 0);


        }
        return Vector4.zero;
    }
}
