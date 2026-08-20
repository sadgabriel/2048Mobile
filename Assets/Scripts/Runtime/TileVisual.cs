using UnityEngine;
using UnityEngine.UI;

namespace Game2048.Runtime
{
    internal sealed class TileVisual
    {
        private readonly RectTransform _rect;
        private readonly Text _text;

        public TileVisual(int tileId, Transform parent, Vector2 position, float size, int value, Color color)
        {
            var image = UiFactory.CreateImage($"Tile_{tileId}", parent, color);
            _rect = image.rectTransform;
            _rect.anchorMin = _rect.anchorMax = new Vector2(0.5f, 0.5f);
            _rect.pivot = new Vector2(0.5f, 0.5f);
            _rect.sizeDelta = new Vector2(size, size);
            _rect.anchoredPosition = position;

            _text = UiFactory.CreateText("Value", image.transform, 36, Color.black);
            var textRect = _text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            _text.text = value.ToString();
        }

        public void SetPosition(Vector2 position) => _rect.anchoredPosition = position;

        public void SetScale(float scale) => _rect.localScale = new Vector3(scale, scale, 1f);

        public void Destroy() => Object.Destroy(_rect.gameObject);
    }
}
