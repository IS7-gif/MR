using UnityEngine;

namespace Project.Scripts.Utils
{
#if UNITY_EDITOR
    [DefaultExecutionOrder(-50)]
#endif
    [ExecuteAlways]
    public class FullscreenBackground : MonoBehaviour
    {
        private void Start()
        {
            if (Application.isPlaying)
                Apply();
        }

        private void Apply()
        {
            var mainCamera = Camera.main;
            if (!mainCamera)
                return;

            var sr = GetComponent<SpriteRenderer>();
            if (!sr || !sr.sprite)
                return;

            var camHeight = mainCamera.orthographicSize * 2f;
            var camWidth = camHeight * GetAspect(mainCamera);
            var spriteSize = sr.sprite.bounds.size;

            var scaleX = camWidth / spriteSize.x;
            var scaleY = camHeight / spriteSize.y;
            var uniformScale = Mathf.Max(scaleX, scaleY);

            transform.localScale = new Vector3(uniformScale, uniformScale, 1f);
            transform.position = Vector3.zero;
        }

        private static float GetAspect(Camera cam)
        {
            var h = UnityEngine.Device.Screen.height;
            return h > 0 ? (float)UnityEngine.Device.Screen.width / h : cam.aspect;
        }


#if UNITY_EDITOR
        private void Update()
        {
            Apply();
        }
#endif
    }
}