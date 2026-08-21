using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game2048.Runtime
{
    public class QuitConfirmDialog
    {
        private readonly GameObject _root;

        public QuitConfirmDialog(Transform parent, Action onConfirm, Action onCancel)
        {
            var background = UiFactory.CreateImage("QuitConfirmDialog", parent, new Color(0f, 0f, 0f, 0.6f));
            _root = background.gameObject;

            var rootRect = _root.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var title = UiFactory.CreateText("Title", _root.transform, 44, Color.white);
            title.text = "Quit game?";
            SetAnchoredBox(title.rectTransform, new Vector2(0.5f, 0.6f), new Vector2(700f, 100f));

            var cancelImage = UiFactory.CreateImage("CancelButton", _root.transform, Color.white);
            SetAnchoredBox(cancelImage.rectTransform, new Vector2(0.35f, 0.42f), new Vector2(260f, 90f));
            AddButton(cancelImage, "Cancel", Color.black, onCancel);

            var quitImage = UiFactory.CreateImage("QuitButton", _root.transform, new Color(0.85f, 0.3f, 0.25f));
            SetAnchoredBox(quitImage.rectTransform, new Vector2(0.65f, 0.42f), new Vector2(260f, 90f));
            AddButton(quitImage, "Quit", Color.white, onConfirm);

            _root.SetActive(false);
        }

        public bool IsShown => _root.activeSelf;

        public void Show() => _root.SetActive(true);

        public void Hide() => _root.SetActive(false);

        private static void AddButton(Image image, string label, Color textColor, Action onClick)
        {
            var button = image.gameObject.AddComponent<Button>();
            button.onClick.AddListener(() => onClick());

            var text = UiFactory.CreateText("Label", image.transform, 30, textColor);
            text.text = label;
            var textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }

        private static void SetAnchoredBox(RectTransform rect, Vector2 anchor, Vector2 size)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
        }
    }
}
