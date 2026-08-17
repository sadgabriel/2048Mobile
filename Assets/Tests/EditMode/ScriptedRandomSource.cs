using System.Collections.Generic;
using Game2048.Model;

namespace Game2048.Model.Tests
{
    internal sealed class ScriptedRandomSource : IRandomSource
    {
        private readonly Queue<int> _values;

        public ScriptedRandomSource(params int[] values)
        {
            _values = new Queue<int>(values);
        }

        public int Next(int minInclusive, int maxExclusive) =>
            _values.Count > 0 ? _values.Dequeue() : minInclusive;
    }
}
