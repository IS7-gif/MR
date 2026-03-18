using UnityEngine;

namespace Project.Scripts.Gameplay
{
    public class BoardView : MonoBehaviour
    {
        [Tooltip("SpriteRenderer of the frame that surrounds the board (Draw Mode must be Sliced)")]
        [SerializeField] private SpriteRenderer _frame;
        [Tooltip("SpriteMask used to hide tiles appearing above the board during gravity")]
        [SerializeField] private SpriteMask _spriteMask;


        public void Setup(int width, int height, float cellSize, float framePadding, float maskTopPadding)
        {
            if (_frame)
            {
                _frame.size = new Vector2(
                    width * cellSize + framePadding * 2f,
                    height * cellSize + framePadding * 2f
                );
            }

            if (_spriteMask && _spriteMask.sprite)
            {
                var targetWidth = width * cellSize + framePadding * 2f;
                var targetHeight = (height + maskTopPadding) * cellSize + framePadding;
                var spriteSize = _spriteMask.sprite.bounds.size;

                _spriteMask.transform.localScale = new Vector3(
                    targetWidth / spriteSize.x,
                    targetHeight / spriteSize.y,
                    1f
                );
            }
        }
    }
}