using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{
    public bool canMove = true;
    public float cameraPanSpeed = 10f;
    public float minX, maxX, minY, maxY;
    float vertExtent;
    float horzExtent;

    bool bDragging;
    Vector3 oldPos;
    Vector3 panOrigin;

    void Start()
    {
        CalculateCameraBounds();
        ClampBounds();


    }

    void CalculateCameraBounds()
    {
        vertExtent = GetComponent<Camera>().orthographicSize;
        horzExtent = vertExtent * Screen.width / Screen.height;
    }

    private void OnValidate()
    {
        ClampBounds();
    }

    void LateUpdate()
    {
        if (canMove)
        {
            MoveCamera();
        }
        ClampBounds();
        var v3 = transform.position;
        v3.x = Mathf.Clamp(v3.x, minX + horzExtent, maxX - horzExtent);
        v3.y = Mathf.Clamp(v3.y, minY + vertExtent, maxY - vertExtent);
        transform.position = v3;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(minX, minY), new Vector2(maxX, minY));//Bottom
        Gizmos.DrawLine(new Vector2(minX, maxY), new Vector2(maxX, maxY));//Top
        Gizmos.DrawLine(new Vector2(minX, minY), new Vector2(minX, maxY));//Left
        Gizmos.DrawLine(new Vector2(maxX, minY), new Vector2(maxX, maxY));//Right
    }

    void ClampBounds()
    {
        minX = MathOperations.ClampMax(minX, maxX);
        maxX = MathOperations.ClampMin(maxX, minX);
        minY = MathOperations.ClampMax(minY, maxY);
        maxY = MathOperations.ClampMin(maxY, minY);
    }

    public void MoveCamera()
    {
        //Check if the drag is not on UI element.
        if (Input.GetMouseButtonDown(0) && !bDragging && !EventSystem.current.IsPointerOverGameObject(-1)
            && !EventSystem.current.IsPointerOverGameObject(0))
        {
            bDragging = true;
            oldPos = transform.position;
            panOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition); //Get the ScreenVector the mouse clicked
        }

        if (Input.GetMouseButton(0) && bDragging)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - panOrigin; //Get the difference between where the mouse clicked and where it moved
            transform.position = oldPos + -pos * cameraPanSpeed;  //Move the position of the camera to simulate a drag, speed * 10 for screen to worldspace conversion
        }

        if (Input.GetMouseButtonUp(0))
        {
            bDragging = false;
        }

    }
}
