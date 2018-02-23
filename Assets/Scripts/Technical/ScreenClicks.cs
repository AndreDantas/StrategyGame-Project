using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenClicks : MonoBehaviour
{
    public static ScreenClicks instance;
    public delegate void OnClickEventHandler(Vector2 originPos, Vector2 releasePos);
    public event OnClickEventHandler OnClick;
    public float clickMaxTime = 0.2f;
    float clickTime;
    Vector2 mouseClickOriginPos;
    Vector2 mouseClickReleasePos;
    bool clicking;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

    }



    private void Update()
    {
        CheckForClick();
    }

    /// <summary>
    /// Checks for a mouse click or a touch on the screen.
    /// </summary>
    public void CheckForClick()
    {
        if (Input.GetMouseButtonDown(0) && !clicking)
        {
            clicking = true;
            clickTime = 0;
            mouseClickOriginPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            clickTime += Time.deltaTime;
            if (clickTime > clickMaxTime)
                clicking = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseClickReleasePos = Input.mousePosition;

            if (clicking && OnClick != null)
                OnClick(mouseClickOriginPos, mouseClickReleasePos);

            clicking = false;
        }
    }

    public Vector2 GetClickOriginPos()
    {
        return mouseClickOriginPos;
    }

    public Vector2 GetClickReleasePos()
    {
        return mouseClickReleasePos;
    }

    /// <summary>
    /// Checks if the pointer was over a UI element.
    /// </summary>
    /// <returns></returns>
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
