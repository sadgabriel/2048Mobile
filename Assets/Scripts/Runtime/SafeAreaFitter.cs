using UnityEngine;

namespace Game2048.Runtime
{
    public class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform _rect;
        private Rect _lastSafeArea;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            Apply();
        }

        private void Update()
        {
            // Cheap to check every frame; only writes to the RectTransform when the
            // safe area actually changes (orientation change, foldable resize, etc.).
            if (Screen.safeArea != _lastSafeArea)
                Apply();
        }

        private void Apply()
        {
            var safeArea = Screen.safeArea;
            _lastSafeArea = safeArea;

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _rect.anchorMin = anchorMin;
            _rect.anchorMax = anchorMax;
        }
    }
}
