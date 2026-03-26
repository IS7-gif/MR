using UnityEngine;

namespace Project.Scripts.Utils
{
#if UNITY_EDITOR
    [DefaultExecutionOrder(-100)]
#endif
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class CameraSetup : MonoBehaviour
    {
        [Tooltip("Reference aspect ratio (width / height). Must match BoardConfig.MaxAspectRatio (default 0.5 for 1:2).")]
        [Range(0.3f, 1f)]
        [SerializeField] private float _referenceAspect = 0.5f;

        [Tooltip("Orthographic size for the reference aspect ratio. Copy the value currently set on the Camera component here.")]
        [SerializeField] private float _referenceOrthographicSize = 5f;

        private Camera _camera;


        private void Awake()
        {
            _camera = GetComponent<Camera>();
            Apply();
        }

        private void Apply()
        {
            if (!_camera)
                _camera = GetComponent<Camera>();

            if (!_camera || !_camera.orthographic)
                return;

            var screenAspect = (float)Screen.width / Screen.height;
            _camera.orthographicSize = _referenceOrthographicSize
                                       * Mathf.Max(1f, _referenceAspect / screenAspect);
        }


#if UNITY_EDITOR
        private void Update()
        {
            Apply();
        }

        [ContextMenu("Capture Current Size As Reference")]
        private void CaptureCurrentSize()
        {
            if (!_camera)
                _camera = GetComponent<Camera>();

            if (_camera)
                _referenceOrthographicSize = _camera.orthographicSize;
        }
#endif
    }
}