using UnityEngine;

namespace Scripts {
    [RequireComponent(typeof(Camera))]
    public class ControlCamera : MonoBehaviour {
        [Range(.01f, .1f)]
        public float sensitivity;
        [Range(.1f, 1f)]
        public float scrollSensitivity;
        public float maxScroll, minScroll;

        new Camera camera;
        Vector3? previous;

        void Start() {
            camera = GetComponent<Camera>();
        }

        void Update() {
            // camera movement
            if (Input.GetMouseButton(0)) {
                if (!previous.HasValue) {
                    previous = Input.mousePosition;
                }
                var dif = (previous.Value - Input.mousePosition) * sensitivity;
                transform.position += new Vector3(dif.x, dif.y, 0);
                previous = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0)) {
                previous = null;
            }

            // zoom
            if (Input.mouseScrollDelta.y != 0) {
                var scroll = Input.mouseScrollDelta.y * scrollSensitivity;
                var size = Mathf.Clamp(camera.orthographicSize - scroll, minScroll, maxScroll);
                camera.orthographicSize = size;
            }
        }
    }
}
