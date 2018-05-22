using Models.Map;
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

        private Vector3 cameraPosition;

        private int minCameraX = 0, maxCameraX;
        private int minCameraY = 0, maxCameraY;

        private void Start()
        {
            camera = Camera.main;
            cameraTransform = camera.GetComponent<Transform>();

            maxCameraX = World.Instance.Width;
            maxCameraY = World.Instance.Height;

            cameraTransform.position = new Vector3(maxCameraX / 2, maxCameraY / 2, cameraTransform.position.z);
        }

        private void LateUpdate()
        {
            currentMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);

            HandleCameraMovement();
            HandleCameraZoom();

            previousMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);

            cameraPosition = cameraTransform.position;
            cameraPosition.x = Mathf.Clamp(cameraPosition.x, minCameraX, maxCameraX);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y, minCameraY, maxCameraY);
            cameraTransform.position = cameraPosition;
        }

        private void HandleCameraMovement()
        {
            if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
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
