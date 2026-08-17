using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game2048.Runtime
{
    public class GameOverPanel
    {
        private readonly GameObject _root;
        private readonly Text _scoreText;

        public GameOverPanel(Transform parent, Action onRestart)
        {
            var background = UiFactory.CreateImage("GameOverPanel", parent, new Color(0f, 0f, 0f, 0.6f));
            _root = background.gameObject;

            var rootRect = _root.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var title = UiFactory.CreateText("Title", _root.transform, 64, Color.white);
            title.text = "Game Over";
            SetAnchoredBox(title.rectTransform, new Vector2(0.5f, 0.6f), new Vector2(600f, 100f));

            _scoreText = UiFactory.CreateText("Score", _root.transform, 40, Color.white);
            SetAnchoredBox(_scoreText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(600f, 80f));

            var buttonImage = UiFactory.CreateImage("RestartButton", _root.transform, new Color(0.93f, 0.76f, 0.18f));
            SetAnchoredBox(buttonImage.rectTransform, new Vector2(0.5f, 0.4f), new Vector2(300f, 90f));

            var button = buttonImage.gameObject.AddComponent<Button>();
            button.onClick.AddListener(() => onRestart());

            var buttonLabel = UiFactory.CreateText("Label", buttonImage.transform, 32, Color.black);
            buttonLabel.text = "Restart";
            var labelRect = buttonLabel.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            _root.SetActive(false);
        }

        public void Show(int finalScore)
        {
            _scoreText.text = $"Final Score: {finalScore}";
            _root.SetActive(true);
        }

        public void Hide() => _root.SetActive(false);

        private static void SetAnchoredBox(RectTransform rect, Vector2 anchor, Vector2 size)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
        }
    }
}
