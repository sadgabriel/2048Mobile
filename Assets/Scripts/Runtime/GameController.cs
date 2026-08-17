using Game2048.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Game2048.Runtime
{
    public class GameController : MonoBehaviour
    {
        private Board _board;
        private GridView _gridView;
        private ScoreDisplay _scoreDisplay;
        private GameOverPanel _gameOverPanel;
        private WinBanner _winBanner;

        private bool _isGameOver;
        private bool _winBannerShown;

        private void Start()
        {
            EnsureEventSystem();
            var canvas = CreateCanvas();

            _gridView = new GridView(canvas.transform);
            _scoreDisplay = new ScoreDisplay(canvas.transform);
            _gameOverPanel = new GameOverPanel(canvas.transform, RestartGame);
            _winBanner = new WinBanner(canvas.transform, DismissWinBanner);

            StartNewGame();
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
            RefreshView();

            if (!_winBannerShown && _board.HasWon)
            {
                _winBannerShown = true;
                _winBanner.Show();
            }

            if (_board.IsGameOver())
            {
                _isGameOver = true;
                _gameOverPanel.Show(_board.Score);
            }
        }

        private void StartNewGame()
        {
            _board = new Board();
            _board.SpawnRandomTile();
            _board.SpawnRandomTile();

            _isGameOver = false;
            _winBannerShown = false;
            _gameOverPanel.Hide();
            _winBanner.Hide();

            RefreshView();
        }

        private void RestartGame() => StartNewGame();

        private void DismissWinBanner() => _winBanner.Hide();

        private void RefreshView()
        {
            _gridView.Render(_board);
            _scoreDisplay.SetScore(_board.Score);
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

        private static Canvas CreateCanvas()
        {
            var canvasGO = new GameObject(
                "GameCanvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));

            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);

            return canvas;
        }

        private static void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null)
                return;

            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }
    }
}
