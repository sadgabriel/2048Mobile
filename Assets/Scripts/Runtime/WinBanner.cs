using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game2048.Runtime
{
    public class WinBanner
    {
        private readonly GameObject _root;

        public WinBanner(Transform parent, Action onDismiss)
        {
            var background = UiFactory.CreateImage("WinBanner", parent, new Color(0.93f, 0.76f, 0.18f, 0.95f));
            _root = background.gameObject;

            var rootRect = _root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.sizeDelta = new Vector2(700f, 220f);
            rootRect.anchoredPosition = Vector2.zero;

            var title = UiFactory.CreateText("Title", _root.transform, 44, Color.white);
            title.text = "You reached 2048!";
            var titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0f, 0.45f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            var buttonImage = UiFactory.CreateImage("KeepPlayingButton", _root.transform, Color.white);
            var buttonRect = buttonImage.rectTransform;
            buttonRect.anchorMin = new Vector2(0.5f, 0f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.45f);
            buttonRect.offsetMin = new Vector2(-150f, 15f);
            buttonRect.offsetMax = new Vector2(150f, -15f);

            var button = buttonImage.gameObject.AddComponent<Button>();
            button.onClick.AddListener(() => onDismiss());

            var label = UiFactory.CreateText("Label", buttonImage.transform, 28, Color.black);
            label.text = "Keep Playing";
            var labelRect = label.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            _root.SetActive(false);
        }

        public void Show() => _root.SetActive(true);

        public void Hide() => _root.SetActive(false);
    }
}
