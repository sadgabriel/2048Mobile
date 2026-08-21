using Game2048.Model;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game2048.Runtime
{
    public class InputReader
    {
        private const float MinSwipeDistance = 50f;

        private bool _isDragging;
        private Vector2 _dragStart;

        public bool TryReadDirection(out Direction direction)
        {
            return TryReadKeyboard(out direction) || TryReadSwipe(out direction);
        }

        // On Android, the system back button/gesture is delivered to Unity as Escape.
        public bool BackPressed()
        {
            var keyboard = Keyboard.current;
            return keyboard != null && keyboard.escapeKey.wasPressedThisFrame;
        }

        private static bool TryReadKeyboard(out Direction direction)
        {
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.upArrowKey.wasPressedThisFrame) { direction = Direction.Up; return true; }
                if (keyboard.downArrowKey.wasPressedThisFrame) { direction = Direction.Down; return true; }
                if (keyboard.leftArrowKey.wasPressedThisFrame) { direction = Direction.Left; return true; }
                if (keyboard.rightArrowKey.wasPressedThisFrame) { direction = Direction.Right; return true; }
            }

            direction = default;
            return false;
        }

        // Pointer covers both mouse (editor drag testing) and touch (device) through
        // the same low-level control, so one code path handles both.
        private bool TryReadSwipe(out Direction direction)
        {
            direction = default;
            var pointer = Pointer.current;
            if (pointer == null)
                return false;

            if (pointer.press.wasPressedThisFrame)
            {
                _isDragging = true;
                _dragStart = pointer.position.ReadValue();
                return false;
            }

            if (!_isDragging || !pointer.press.wasReleasedThisFrame)
                return false;

            _isDragging = false;
            var delta = pointer.position.ReadValue() - _dragStart;
            if (delta.magnitude < MinSwipeDistance)
                return false;

            direction = ClassifyDirection(delta);
            return true;
        }

        private static Direction ClassifyDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                return delta.x > 0 ? Direction.Right : Direction.Left;

            return delta.y > 0 ? Direction.Up : Direction.Down;
        }
    }
}
