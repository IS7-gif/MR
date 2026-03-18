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
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorApplication.delayCall += () =>
                {
                    if (this) 
                        Apply();
                };
                
                return;
            }
#endif
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

            var aspect = GetAspect(mainCamera);
            var camHeight = mainCamera.orthographicSize * 2f;
            var camWidth = camHeight * aspect;
            var spriteSize = sr.sprite.bounds.size;

            var scaleX = camWidth / spriteSize.x;
            var scaleY = camHeight / spriteSize.y;
            var uniformScale = Mathf.Max(scaleX, scaleY);

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
                "GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            if (null == getSizeOfMainGameView)
                return Vector2.zero;

            return (Vector2)getSizeOfMainGameView.Invoke(null, null);
        }
#endif
    }
}