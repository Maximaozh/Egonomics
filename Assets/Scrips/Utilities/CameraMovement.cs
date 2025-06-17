using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float touchZoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private GameObject target;
    [SerializeField] private Vector2 minBounds = new Vector2(-25, -25);
    [SerializeField] private Vector2 maxBounds = new Vector2(25, 25);

    [SerializeField] private Vector2 lastTouchPosition;
    [SerializeField] private float lastPinchDistance;

    [SerializeField] private bool uiIsOpen = false;
    [SerializeField] private GameObject dialogWindow;

    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float ZoomSpeed { get => zoomSpeed; set => zoomSpeed = value; }
    public float TouchZoomSpeed { get => touchZoomSpeed; set => touchZoomSpeed = value; }
    public float MinZoom { get => minZoom; set => minZoom = value; }
    public float MaxZoom { get => maxZoom; set => maxZoom = value; }
    public GameObject Target { get => target; set => target = value; }
    public Vector2 MinBounds { get => minBounds; set => minBounds = value; }
    public Vector2 MaxBounds { get => maxBounds; set => maxBounds = value; }
    public bool UiIsOpen { get => uiIsOpen; set => uiIsOpen = value; }
    public GameObject DialogWindow { get => dialogWindow; set => dialogWindow = value; }

    void Update()
    {
        if (UiIsOpen)
            return;

        HandleMovement();
        HandleZoom();
    }

    void LateUpdate()
    {
        for (int i = 0; i < DialogWindow.transform.childCount; i++)
        {
            if (DialogWindow.transform.GetChild(i).gameObject.activeSelf)
            {
                UiIsOpen = true;
                break;
            }
            else
            {
                UiIsOpen = false;
            }
        }

        if (Target != null && !UiIsOpen)
        {
            Vector3 targetPosition = Target.transform.position;
            float clampedX = Mathf.Clamp(targetPosition.x, MinBounds.x, MaxBounds.x);
            float clampedY = Mathf.Clamp(targetPosition.y, MinBounds.y, MaxBounds.y);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
    }

    private void HandleMovement()
    {
        Vector3 movement = Vector3.zero;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        movement += new Vector3(horizontal, vertical, 0);

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 delta = touch.deltaPosition;
            movement += new Vector3(-delta.x, -delta.y, 0) * 0.05f;
        }

        if (movement.magnitude > 0)
        {
            transform.Translate(movement * MoveSpeed * Time.deltaTime);
        }
    }

    private void HandleZoom()
    {
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if (zoom != 0)
        {
            Camera.main.orthographicSize = Mathf.Clamp(
                Camera.main.orthographicSize - zoom * ZoomSpeed,
                MinZoom,
                MaxZoom);
        }

        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                lastPinchDistance = Vector2.Distance(touch0.position, touch1.position);
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touch0.position, touch1.position);
                float delta = currentDistance - lastPinchDistance;

                Camera.main.orthographicSize = Mathf.Clamp(
                    Camera.main.orthographicSize - delta * TouchZoomSpeed,
                    MinZoom,
                    MaxZoom);

                lastPinchDistance = currentDistance;
            }
        }
    }
}