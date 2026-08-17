using System.Collections.Generic;
using Game2048.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Game2048.Runtime
{
    public class GridView
    {
        private const float CellSize = 150f;
        private const float CellSpacing = 12f;

        private static readonly Color EmptyCellColor = new Color(0.80f, 0.75f, 0.70f);

        private static readonly Dictionary<int, Color> TileColors = new Dictionary<int, Color>
        {
            { 2, new Color(0.93f, 0.89f, 0.85f) },
            { 4, new Color(0.93f, 0.88f, 0.78f) },
            { 8, new Color(0.95f, 0.69f, 0.47f) },
            { 16, new Color(0.96f, 0.58f, 0.39f) },
            { 32, new Color(0.96f, 0.49f, 0.37f) },
            { 64, new Color(0.96f, 0.37f, 0.23f) },
            { 128, new Color(0.93f, 0.81f, 0.45f) },
            { 256, new Color(0.93f, 0.80f, 0.38f) },
            { 512, new Color(0.93f, 0.78f, 0.31f) },
            { 1024, new Color(0.93f, 0.77f, 0.25f) },
            { 2048, new Color(0.93f, 0.76f, 0.18f) },
        };

        private readonly Image[,] _cellImages = new Image[Board.Size, Board.Size];
        private readonly Text[,] _cellTexts = new Text[Board.Size, Board.Size];

        public GridView()
        {
            var canvasGO = new GameObject("GameCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);

            var background = CreateUIObject("Background", canvasGO.transform);
            background.AddComponent<Image>().color = new Color(0.73f, 0.68f, 0.63f);

            var layout = background.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(CellSize, CellSize);
            layout.spacing = new Vector2(CellSpacing, CellSpacing);
            layout.padding = new RectOffset((int)CellSpacing, (int)CellSpacing, (int)CellSpacing, (int)CellSpacing);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = Board.Size;

            var totalSize = Board.Size * CellSize + (Board.Size + 1) * CellSpacing;
            var backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.sizeDelta = new Vector2(totalSize, totalSize);
            backgroundRect.anchorMin = backgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRect.anchoredPosition = Vector2.zero;

            // GridLayoutGroup fills children left-to-right then top-to-bottom, so this
            // loop order (y outer, x inner) must match to land each cell at the right (x,y).
            for (var y = 0; y < Board.Size; y++)
            for (var x = 0; x < Board.Size; x++)
            {
                var cellGO = CreateUIObject($"Cell_{x}_{y}", background.transform);
                var cellImage = cellGO.AddComponent<Image>();
                cellImage.color = EmptyCellColor;

                var textGO = CreateUIObject("Value", cellGO.transform);
                var text = textGO.AddComponent<Text>();
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                text.fontSize = 36;
                text.alignment = TextAnchor.MiddleCenter;
                text.color = Color.black;

                var textRect = text.rectTransform;
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;

                _cellImages[x, y] = cellImage;
                _cellTexts[x, y] = text;
            }
        }

        public void Render(Board board)
        {
            for (var y = 0; y < Board.Size; y++)
            for (var x = 0; x < Board.Size; x++)
            {
                var tile = board.GetTile(x, y);
                var image = _cellImages[x, y];
                var text = _cellTexts[x, y];

                if (tile == null)
                {
                    image.color = EmptyCellColor;
                    text.text = string.Empty;
                }
                else
                {
                    image.color = TileColors.TryGetValue(tile.Value, out var color) ? color : Color.magenta;
                    text.text = tile.Value.ToString();
                }
            }
        }

        private static GameObject CreateUIObject(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }
    }
}
