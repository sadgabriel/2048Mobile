using System.Collections;
using System.Collections.Generic;
using Game2048.Model;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private QuitConfirmDialog _quitConfirmDialog;
        private InputReader _inputReader;

        private bool _isGameOver;
        private bool _winBannerShown;
        private bool _isAnimating;

        private void Start()
        {
            // Without an explicit target, some OEM Android skins throttle
            // unrecognized apps to a low refresh rate regardless of vSyncCount.
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            EnsureEventSystem();
            var canvas = CreateCanvas();
            var safeArea = CreateSafeArea(canvas);

            _gridView = new GridView(safeArea);
            _scoreDisplay = new ScoreDisplay(safeArea);
            _gameOverPanel = new GameOverPanel(safeArea, RestartGame);
            _winBanner = new WinBanner(safeArea, DismissWinBanner);
            _quitConfirmDialog = new QuitConfirmDialog(safeArea, ConfirmQuit, CancelQuit);
            _inputReader = new InputReader();

            StartNewGame();
        }

        private void Update()
        {
            if (_inputReader.BackPressed())
            {
                if (_quitConfirmDialog.IsShown)
                    _quitConfirmDialog.Hide();
                else
                    _quitConfirmDialog.Show();
                return;
            }

            if (_isGameOver || _isAnimating || _quitConfirmDialog.IsShown)
                return;

            if (!_inputReader.TryReadDirection(out var direction))
                return;

            var result = _board.Move(direction, out var movements);
            if (!result.Moved)
                return;

            StartCoroutine(PlayMoveSequence(movements));
        }

        private IEnumerator PlayMoveSequence(IReadOnlyList<TileMovement> movements)
        {
            _isAnimating = true;

            yield return _gridView.AnimateMove(_board, movements);

            _board.SpawnRandomTile();
            yield return _gridView.AnimateSpawn(_board);

            _scoreDisplay.SetScore(_board.Score);

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

            _isAnimating = false;
        }

        private void StartNewGame()
        {
            _board = new Board();
            _board.SpawnRandomTile();
            _board.SpawnRandomTile();

            _isGameOver = false;
            _winBannerShown = false;
            _isAnimating = false;
            _gameOverPanel.Hide();
            _winBanner.Hide();
            _quitConfirmDialog.Hide();

            _scoreDisplay.SetScore(_board.Score);
            _gridView.SyncInstant(_board);
        }

        private void RestartGame() => StartNewGame();

        private void DismissWinBanner() => _winBanner.Hide();

        private void CancelQuit() => _quitConfirmDialog.Hide();

        private static void ConfirmQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
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
            scaler.matchWidthOrHeight = 0.5f;

            return canvas;
        }

        private static Transform CreateSafeArea(Canvas canvas)
        {
            var go = new GameObject("SafeArea", typeof(RectTransform));
            go.transform.SetParent(canvas.transform, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            go.AddComponent<SafeAreaFitter>();

            return go.transform;
        }

        private static void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null)
                return;

            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }
    }
}
