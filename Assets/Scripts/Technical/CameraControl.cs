using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{
    public static CameraControl instance;
    public Transform followTarget;
    public Vector3 followVelocity
    {
        get
        {
            return velocity;
        }
    }
    public bool canMove = true;
    public bool canZoom = false;
    [ConditionalHide("canZoom", true)]
    public float minZoom = 5;
    [ConditionalHide("canZoom", true)]
    public float maxZoom = 12f;
    public float cameraPanSpeed = 10f;
    public float followSmoothTime = 0.5f;
    public float minX, maxX, minY, maxY;
    float vertExtent;
    float horzExtent;
    bool moving;
    bool bDragging;
    PixelPerfectCamera pixelPerfectCam;
    Camera cam;
    Vector3 oldPos;
    Vector3 panOrigin;
    Vector3 velocity;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        cam = GetComponent<Camera>();
        pixelPerfectCam = GetComponent<PixelPerfectCamera>();
        instance = this;
    }
    void Start()
    {
        CalculateCameraBounds();
        ClampBounds();

    }

    void CalculateCameraBounds()
    {
        vertExtent = cam.orthographicSize;
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
            if (Application.isEditor)
                CameraMovementMouse();
            else
                CameraMovementMobile();
        }
        ClampBounds();
        var v3 = transform.position;
        v3.x = Mathf.Clamp(v3.x, minX + horzExtent, maxX - horzExtent);
        v3.y = Mathf.Clamp(v3.y, minY + vertExtent, maxY - vertExtent);
        transform.position = v3;
    }

    public void FixedUpdate()
    {
        CameraFollow();
    }

    public void StartFollow(Transform follow)
    {
        followTarget = follow;
        canMove = false;
    }

    public void StopFollow()
    {
        followTarget = null;
        canMove = true;
    }
    public void SetCameraMovement(bool canMove)
    {
        this.canMove = canMove;
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

    public bool InsideBounds()
    {

        if (transform.position.x >= minX + horzExtent && transform.position.x <= maxX - horzExtent)
            if (transform.position.y >= minY + vertExtent && transform.position.y <= maxY - vertExtent)
                return true;

        return false;

    }

    public void MoveCameraToPos(Vector2 pos, float time = 0.5f)
    {
        if (moving)
            return;
        StartCoroutine(MoveCamera(pos, time));
    }

    public void CameraFollow()
    {
        if (followTarget)
        {
            Vector3 newPos = new Vector3(followTarget.position.x, followTarget.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, followSmoothTime);
        }
    }

    IEnumerator MoveCamera(Vector2 pos, float time = 0.5f)
    {
        moving = true;
        canMove = false;
        time = MathOperations.ClampMin(time, 0);
        Vector3 newPos = new Vector3(pos.x, pos.y, transform.position.z);
        float t = 0;
        float lerptime = 0;
        Vector3 origin = transform.position;
        while (t < 0.99f && lerptime < time)
        {
            t = lerptime / time;
            t = MathOperations.ChangeLerpT(LerpMode.EaseOut, t);
            transform.position = Vector3.Lerp(origin, newPos, t);
            lerptime += Time.deltaTime;
            if (Vector2.Distance(transform.position, newPos) < 0.01f)
                break;
            yield return null;
        }
        transform.position = newPos;
        canMove = true;
        moving = false;
    }

    public bool IsMoving()
    {
        return moving;
    }

    public void CameraMovementMouse()
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
            float panSpeedMulti = MathOperations.Map(minZoom, maxZoom, 1f, 2f, pixelPerfectCam.targetCameraHalfHeight);
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - panOrigin; //Get the difference between where the mouse clicked and where it moved
            transform.position = oldPos + -pos * cameraPanSpeed * panSpeedMulti;  //Move the position of the camera to simulate a drag, speed * 10 for screen to worldspace conversion
        }

        if (Input.GetMouseButtonUp(0))
        {
            bDragging = false;
        }

        if (Input.mouseScrollDelta.y != 0 && canZoom)
        {
            ZoomCamera(Input.mouseScrollDelta.y, 1f);
        }
    }

    public void CameraMovementMobile()
    {
        float touchCount = Input.touchCount;
        // Get Input from Mobile Device

        if (touchCount == 1)
        {
            float panSpeedMulti = MathOperations.Map(minZoom, maxZoom, 1f, 2f, pixelPerfectCam.targetCameraHalfHeight);
            if (Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(-1)
                && !EventSystem.current.IsPointerOverGameObject(0))
            {
                oldPos = transform.position;
                panOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition); //Get the ScreenVector the mouse clicked
            }

            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - panOrigin; //Get the difference between where the mouse clicked and where it moved
                transform.position = oldPos + -pos * cameraPanSpeed * panSpeedMulti;  //Move the position of the camera to simulate a drag, speed * 10 for screen to worldspace conversion
            }

        }
        // Check Pinching to Zoom in - out on Mobile device
        if (touchCount == 2 && canZoom)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevTouchDelta = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDelta = (touch0.position - touch1.position).magnitude;

            float zoomDelta = prevTouchDelta - touchDelta;
            ZoomCamera(-zoomDelta, 0.01f);
        }


    }

    void ZoomCamera(float offset, float speed)
    {
        if (offset == 0)
        {
            return;
        }


        pixelPerfectCam.targetCameraHalfHeight = Mathf.Clamp(pixelPerfectCam.targetCameraHalfHeight - (offset * speed), minZoom, maxZoom);
        pixelPerfectCam.adjustCameraFOV();
        CalculateCameraBounds();
    }
}
