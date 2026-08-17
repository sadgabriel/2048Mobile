using Game2048.Model;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game2048.Runtime
{
    public class GameController : MonoBehaviour
    {
        private Board _board;
        private GridView _gridView;
        private bool _isGameOver;

        private void Start()
        {
            _gridView = new GridView();

            _board = new Board();
            _board.SpawnRandomTile();
            _board.SpawnRandomTile();
            _gridView.Render(_board);
        }

        private void Update()
        {
            if (_isGameOver)
                return;

            var direction = ReadDirectionInput();
            if (direction == null)
                return;

            var result = _board.Move(direction.Value);
            if (!result.Moved)
                return;

            _board.SpawnRandomTile();
            _gridView.Render(_board);
            Debug.Log($"Score: {_board.Score}");

            if (_board.IsGameOver())
            {
                _isGameOver = true;
                Debug.Log($"Game Over — Final Score: {_board.Score}");
            }
        }

        private static Direction? ReadDirectionInput()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
                return null;

            if (keyboard.upArrowKey.wasPressedThisFrame) return Direction.Up;
            if (keyboard.downArrowKey.wasPressedThisFrame) return Direction.Down;
            if (keyboard.leftArrowKey.wasPressedThisFrame) return Direction.Left;
            if (keyboard.rightArrowKey.wasPressedThisFrame) return Direction.Right;
            return null;
        }
    }
}
