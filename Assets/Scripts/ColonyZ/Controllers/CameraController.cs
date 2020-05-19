using ColonyZ.Models.Map;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ColonyZ.Controllers
{
    public class CameraController : MonoBehaviour
    {
        private new Camera camera;

        public float cameraMoveSpeed = 1.0f;

        [SerializeField] private Transform cameraPivot;

        private Vector3 cameraPosition;

        private Vector2 currentMousePosition;

        public float maxZoom = 5.0f;

        private int minCameraX = 0, maxCameraX;
        private int minCameraY = 0, maxCameraY;
        public float minZoom = 15.0f;
        private Vector2 previousMousePosition;
        public float zoomSpeed = 1.5f;

        private void Start()
        {
            camera = Camera.main;

            maxCameraX = World.Instance.Width;
            maxCameraY = World.Instance.Height;

            cameraPivot.position = new Vector3(maxCameraX / 2, maxCameraY / 2, cameraPivot.position.z);
        }

        private void LateUpdate()
        {
            currentMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);

            HandleCameraMovement();
            HandleCameraZoom();

            previousMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);

            cameraPosition = cameraPivot.position;
            cameraPosition.x = Mathf.Clamp(cameraPosition.x, minCameraX, maxCameraX);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y, minCameraY, maxCameraY);
            cameraPivot.position = cameraPosition;
        }

        private void HandleCameraMovement()
        {
            if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
                cameraPivot.Translate(previousMousePosition - currentMousePosition);

            cameraPivot.Translate(Vector3.right * (Input.GetAxis("Horizontal") *
                                                   (cameraMoveSpeed * camera.orthographicSize * Time.deltaTime)));
            cameraPivot.Translate(Vector3.up * (Input.GetAxis("Vertical") *
                                                (cameraMoveSpeed * camera.orthographicSize * Time.deltaTime)));
        }

        private void HandleCameraZoom()
        {
            // Don't zoom camera when scrolling over UI elements.
            if (EventSystem.current.IsPointerOverGameObject()) return;

            camera.orthographicSize -= camera.orthographicSize * Input.GetAxis("Mouse ScrollWheel") *
                                       (zoomSpeed * Time.deltaTime);
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, maxZoom, minZoom);
        }
    }
}