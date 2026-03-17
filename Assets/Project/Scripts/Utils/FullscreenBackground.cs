using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Project.Scripts.Utils
{
    [ExecuteAlways]
    public class FullscreenBackground : MonoBehaviour
    {
        private void Awake()
        {
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

            float aspect = GetAspect(mainCamera);
            float camHeight = mainCamera.orthographicSize * 2f;
            float camWidth = camHeight * aspect;

            Vector2 spriteSize = sr.sprite.bounds.size;

            float scaleX = camWidth / spriteSize.x;
            float scaleY = camHeight / spriteSize.y;
            float uniformScale = Mathf.Max(scaleX, scaleY);

            transform.localScale = new Vector3(uniformScale, uniformScale, 1f);
            transform.position = Vector3.zero;
        }

        private float GetAspect(Camera cam)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var gameViewSize = GetGameViewSize();
                if (gameViewSize.y > 0)
                    return gameViewSize.x / gameViewSize.y;
            }
#endif
            return cam.aspect;
        }

#if UNITY_EDITOR
        private Vector2 GetGameViewSize()
        {
            var gameViewType = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            if (null == gameViewType)
                return Vector2.zero;

            var window = EditorWindow.GetWindow(gameViewType, false, null, false);
            if (!window)
                return Vector2.zero;

            var getSizeOfMainGameView = gameViewType.GetMethod(
                "GetSizeOfMainGameView",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            if (null == getSizeOfMainGameView)
                return Vector2.zero;

            return (Vector2)getSizeOfMainGameView.Invoke(null, null);
        }
#endif
    }
}