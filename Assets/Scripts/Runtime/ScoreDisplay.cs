using UnityEngine;
using UnityEngine.UI;

namespace Game2048.Runtime
{
    public class ScoreDisplay
    {
        private readonly Text _text;

        public ScoreDisplay(Transform parent)
        {
            _text = UiFactory.CreateText("ScoreDisplay", parent, 48, Color.black);

            var rect = _text.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(600f, 100f);
            rect.anchoredPosition = new Vector2(0f, -80f);

            SetScore(0);
        }

        public void SetScore(int score) => _text.text = $"Score: {score}";
    }
}
