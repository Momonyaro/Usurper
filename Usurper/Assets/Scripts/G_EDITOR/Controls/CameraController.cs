using UnityEngine;

namespace EDITOR.CONTROLS
{
    public class CameraController : MonoBehaviour
    {
        private const float minSize = 7;
        private const float maxSize = 40;

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

            CameraMoveAction(movementVec);
            if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus)) CameraZoomAction(-1);
            if (Input.GetKey(KeyCode.Minus)|| Input.GetKey(KeyCode.KeypadMinus))CameraZoomAction( 1);
        }

        private void CameraZoomAction(float scrollMultiplier)
        {
            float actualScrollSpeed = camZoomSpeed * scrollMultiplier;

            cam.orthographicSize += actualScrollSpeed;
            if (cam.orthographicSize < minSize) { cam.orthographicSize = minSize; }
            if (cam.orthographicSize > maxSize) { cam.orthographicSize = maxSize; }
        }

        private void CameraMoveAction(Vector3 movementVec)
        {
            movementVec *= camMoveSpeed;
            if (movementVec.x < .4f && movementVec.y < .4f) Debug.Log(movementVec);
            Vector3 lerpOffset = cam.transform.position + movementVec;
            cam.transform.position = Vector3.Lerp(cam.transform.position, lerpOffset, camMoveSpeed);
        }

    }
}