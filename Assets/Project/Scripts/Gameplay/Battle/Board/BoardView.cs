using UnityEngine;

namespace Project.Scripts.Gameplay.Battle.Board
{
    public class BoardView : MonoBehaviour
    {
        [Tooltip("SpriteRenderer рамки, окружающей доску (Draw Mode должен быть Sliced)")]
        [SerializeField] private SpriteRenderer _frame;

        [Tooltip("SpriteMask для скрытия тайлов, появляющихся выше доски во время гравитации")]
        [SerializeField] private SpriteMask _spriteMask;


        public void Setup(float frameWidth, float frameHeight, float tileCellSize, float maskTopPadding)
        {
            if (_frame)
            {
                var worldScale = _frame.transform.lossyScale;
                _frame.size = new Vector2(
                    frameWidth  / worldScale.x,
                    frameHeight / worldScale.y
                );
            }

            if (_spriteMask && _spriteMask.sprite)
            {
                var maskExtraHeight = maskTopPadding * tileCellSize;
                var maskHeight = Mathf.Max(0.01f, frameHeight + maskExtraHeight);
                var maskOffsetY = maskExtraHeight * 0.5f;

                var spriteSize = _spriteMask.sprite.bounds.size;
                var parentScale = _spriteMask.transform.parent
                    ? _spriteMask.transform.parent.lossyScale
                    : Vector3.one;

                _spriteMask.transform.localScale = new Vector3(
                    frameWidth / (spriteSize.x * parentScale.x),
                    maskHeight / (spriteSize.y * parentScale.y),
                    1f
                );
                _spriteMask.transform.localPosition = new Vector3(0f, maskOffsetY, 0f);
            }
        }
    }
}