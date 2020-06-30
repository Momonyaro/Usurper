using UnityEngine;
using RENDERER.MAP;

namespace EDITOR.CONTROLS
{
    public class CameraController : MonoBehaviour
    {
        private const float minSize = 7;
        private const float maxSize = 24;

        public float camMoveSpeed = 1.0f;
        public float camZoomSpeed = .01f;
        private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }


        private void FixedUpdate()
        {
            Vector3 movementVec = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

            //CameraMoveAction(movementVec);
            if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus)) CameraZoomAction(-1);
            if (Input.GetKey(KeyCode.Minus)|| Input.GetKey(KeyCode.KeypadMinus))CameraZoomAction( 1);
            DebugKeyboardInput();
        }

        private void CameraZoomAction(float scrollMultiplier)
        {
            float actualScrollSpeed = camZoomSpeed * scrollMultiplier;

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, cam.orthographicSize + actualScrollSpeed, camZoomSpeed);
            if (cam.orthographicSize < minSize) { cam.orthographicSize = minSize; }
            if (cam.orthographicSize > maxSize) { cam.orthographicSize = maxSize; }
        }

        private void CameraMoveAction(Vector3 movementVec)
        {
            movementVec *= camMoveSpeed;
            Vector3 lerpOffset = cam.transform.position + movementVec;
            cam.transform.position = Vector3.Lerp(cam.transform.position, lerpOffset, camMoveSpeed);
        }

        private void DebugKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.F1)) { MapViewport.MAP_DISPLAY_MODE = MAP_DISPLAY_MODES.DEFAULT; FindObjectOfType<MapViewport>().OnMapUpdate(); }
            if (Input.GetKeyDown(KeyCode.F2)) { MapViewport.MAP_DISPLAY_MODE = MAP_DISPLAY_MODES.COLLIDER; FindObjectOfType<MapViewport>().OnMapUpdate(); }
            if (Input.GetKeyDown(KeyCode.F3)) { MapViewport.MAP_DISPLAY_MODE = MAP_DISPLAY_MODES.LIGHTS; FindObjectOfType<MapViewport>().OnMapUpdate(); }
        }

    }
}