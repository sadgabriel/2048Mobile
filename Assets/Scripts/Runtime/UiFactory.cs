using UnityEngine;
using UnityEngine.UI;

namespace Game2048.Runtime
{
    internal static class UiFactory
    {
        public static GameObject CreateUIObject(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        public static Image CreateImage(string name, Transform parent, Color color)
        {
            var image = CreateUIObject(name, parent).AddComponent<Image>();
            image.color = color;
            return image;
        }

        public static Text CreateText(string name, Transform parent, int fontSize, Color color)
        {
            var text = CreateUIObject(name, parent).AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = color;
            return text;
        }
    }
}
