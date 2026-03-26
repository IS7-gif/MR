using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Utils
{
    [ExecuteAlways]
    [RequireComponent(typeof(Image), typeof(AspectRatioFitter))]
    public class SpriteAspectBinder : MonoBehaviour
    {
        private void Awake() => Apply();

        private void Apply()
        {
            var image = GetComponent<Image>();
            if (!image.sprite)
                return;

            var size = image.sprite.bounds.size;
            if (size.y <= 0f)
                return;

            GetComponent<AspectRatioFitter>().aspectRatio = size.x / size.y;
        }


#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
                Apply();
        }
#endif
    }
}