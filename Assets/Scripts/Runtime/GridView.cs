using System.Collections;
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
        private const float SlideDuration = 0.12f;
        private const float PopDuration = 0.1f;

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

        private readonly Transform _tileLayer;
        private readonly Dictionary<int, TileVisual> _visuals = new Dictionary<int, TileVisual>();

        public GridView(Transform parent)
        {
            var totalSize = Board.Size * CellSize + (Board.Size + 1) * CellSpacing;

            var background = UiFactory.CreateImage("Background", parent, new Color(0.73f, 0.68f, 0.63f));
            var layout = background.gameObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(CellSize, CellSize);
            layout.spacing = new Vector2(CellSpacing, CellSpacing);
            layout.padding = new RectOffset((int)CellSpacing, (int)CellSpacing, (int)CellSpacing, (int)CellSpacing);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = Board.Size;

            var backgroundRect = background.rectTransform;
            backgroundRect.sizeDelta = new Vector2(totalSize, totalSize);
            backgroundRect.anchorMin = backgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRect.anchoredPosition = Vector2.zero;

            for (var y = 0; y < Board.Size; y++)
            for (var x = 0; x < Board.Size; x++)
                UiFactory.CreateImage($"CellBackground_{x}_{y}", background.transform, EmptyCellColor);

            // Sibling of Background (not a child) so GridLayoutGroup never touches
            // its children's RectTransforms — tiles here are positioned freely.
            var tileLayerGO = UiFactory.CreateUIObject("TileLayer", parent);
            var tileLayerRect = tileLayerGO.GetComponent<RectTransform>();
            tileLayerRect.sizeDelta = new Vector2(totalSize, totalSize);
            tileLayerRect.anchorMin = tileLayerRect.anchorMax = new Vector2(0.5f, 0.5f);
            tileLayerRect.anchoredPosition = Vector2.zero;
            _tileLayer = tileLayerRect;
        }

        public void SyncInstant(Board board)
        {
            foreach (var visual in _visuals.Values)
                visual.Destroy();
            _visuals.Clear();

            for (var y = 0; y < Board.Size; y++)
            for (var x = 0; x < Board.Size; x++)
            {
                var tile = board.GetTile(x, y);
                if (tile != null)
                    CreateVisual(tile.Id, x, y, tile.Value);
            }
        }

        public IEnumerator AnimateMove(Board board, IReadOnlyList<TileMovement> movements)
        {
            var slides = new List<(TileVisual visual, Vector2 from, Vector2 to)>();
            foreach (var movement in movements)
                if (_visuals.TryGetValue(movement.TileId, out var visual))
                    slides.Add((visual, CellPosition(movement.FromX, movement.FromY), CellPosition(movement.ToX, movement.ToY)));

            var elapsed = 0f;
            while (elapsed < SlideDuration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / SlideDuration);
                foreach (var slide in slides)
                    slide.visual.SetPosition(Vector2.Lerp(slide.from, slide.to, t));
                yield return null;
            }

            foreach (var slide in slides)
                slide.visual.SetPosition(slide.to);

            var mergeDestinations = new HashSet<(int x, int y)>();
            foreach (var movement in movements)
            {
                if (!movement.ConsumedByMerge)
                    continue;

                if (_visuals.TryGetValue(movement.TileId, out var visual))
                {
                    visual.Destroy();
                    _visuals.Remove(movement.TileId);
                }

                mergeDestinations.Add((movement.ToX, movement.ToY));
            }

            var poppingIn = new List<TileVisual>();
            foreach (var (x, y) in mergeDestinations)
            {
                var tile = board.GetTile(x, y);
                if (tile == null || _visuals.ContainsKey(tile.Id))
                    continue;

                var visual = CreateVisual(tile.Id, x, y, tile.Value);
                visual.SetScale(0f);
                poppingIn.Add(visual);
            }

            yield return PopIn(poppingIn);
        }

        public IEnumerator AnimateSpawn(Board board)
        {
            TileVisual spawned = null;
            for (var y = 0; y < Board.Size && spawned == null; y++)
            for (var x = 0; x < Board.Size && spawned == null; x++)
            {
                var tile = board.GetTile(x, y);
                if (tile != null && !_visuals.ContainsKey(tile.Id))
                    spawned = CreateVisual(tile.Id, x, y, tile.Value);
            }

            if (spawned == null)
                yield break;

            spawned.SetScale(0f);
            yield return PopIn(new List<TileVisual> { spawned });
        }

        private static IEnumerator PopIn(List<TileVisual> visuals)
        {
            if (visuals.Count == 0)
                yield break;

            var elapsed = 0f;
            while (elapsed < PopDuration)
            {
                elapsed += Time.deltaTime;
                var scale = Mathf.Clamp01(elapsed / PopDuration);
                foreach (var visual in visuals)
                    visual.SetScale(scale);
                yield return null;
            }

            foreach (var visual in visuals)
                visual.SetScale(1f);
        }

        private TileVisual CreateVisual(int id, int x, int y, int value)
        {
            var color = TileColors.TryGetValue(value, out var c) ? c : Color.magenta;
            var visual = new TileVisual(id, _tileLayer, CellPosition(x, y), CellSize, value, color);
            _visuals[id] = visual;
            return visual;
        }

        private static Vector2 CellPosition(int x, int y)
        {
            var totalSize = Board.Size * CellSize + (Board.Size + 1) * CellSpacing;
            var originX = -totalSize / 2f + CellSpacing + CellSize / 2f;
            var originY = totalSize / 2f - CellSpacing - CellSize / 2f;
            return new Vector2(originX + x * (CellSize + CellSpacing), originY - y * (CellSize + CellSpacing));
        }
    }
}
