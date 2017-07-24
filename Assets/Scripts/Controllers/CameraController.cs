using UnityEngine;

namespace Controllers
{
	public class CameraController : MonoBehaviour
	{

	    public float maxZoom = 5.0f;
	    public float minZoom = 15.0f;
	    public float zoomSpeed = 1.5f;

	    public float cameraMoveSpeed = 1.0f;

	    private new Camera camera;
	    private Transform cameraTransform;

	    private Vector2 currentMousePosition;
	    private Vector2 previousMousePosition;

        private void Start()
        {
            camera = Camera.main;
            cameraTransform = camera.GetComponent<Transform>();
        }

        private void Update()
        {
            currentMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);

            HandleCameraMovement();
            HandleCameraZoom();

            previousMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
        }

        private void HandleCameraMovement()
        {
            if(Input.GetMouseButton(1) || Input.GetMouseButton(2))
            {
                cameraTransform.Translate(previousMousePosition - currentMousePosition);
            }

            cameraTransform.Translate(Vector3.right * (Input.GetAxis("Horizontal") * (cameraMoveSpeed * camera.orthographicSize * Time.deltaTime)));
            cameraTransform.Translate(Vector3.up * (Input.GetAxis("Vertical") * (cameraMoveSpeed * camera.orthographicSize * Time.deltaTime)));
        }

        private void HandleCameraZoom()
        {
            camera.orthographicSize -= camera.orthographicSize * Input.GetAxis("Mouse ScrollWheel") * (zoomSpeed * Time.deltaTime);
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, maxZoom, minZoom);
        }
	}
}
