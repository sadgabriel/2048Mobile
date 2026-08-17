using Game2048.Model;
using NUnit.Framework;

namespace Game2048.Model.Tests
{
    public class SpawnLogicTests
    {
        [Test]
        public void SpawnRandomTile_PlacesTileAtScriptedEmptyCellIndex()
        {
            var board = new Board(new ScriptedRandomSource(0, 0));
            for (var y = 0; y < Board.Size; y++)
            for (var x = 0; x < Board.Size; x++)
                if (!(y == 0 && (x == 0 || x == 1)))
                    board.Seed(x, y, 2);

            var spawned = board.SpawnRandomTile();

            Assert.IsTrue(spawned);
            Assert.IsNotNull(board.GetTile(0, 0));
            Assert.IsNull(board.GetTile(1, 0));
        }

        [Test]
        public void SpawnRandomTile_UsesLowRollForValueTwo()
        {
            var board = new Board(new ScriptedRandomSource(0, 0));

            board.SpawnRandomTile();

            Assert.AreEqual(2, board.GetTile(0, 0).Value);
        }

        [Test]
        public void SpawnRandomTile_UsesHighRollForValueFour()
        {
            var board = new Board(new ScriptedRandomSource(0, 9));

            board.SpawnRandomTile();

            Assert.AreEqual(4, board.GetTile(0, 0).Value);
        }

        [Test]
        public void SpawnRandomTile_ReturnsFalseWhenBoardIsFull()
        {
            var board = new Board(new ScriptedRandomSource());
            for (var y = 0; y < Board.Size; y++)
            for (var x = 0; x < Board.Size; x++)
                board.Seed(x, y, 2);

            var spawned = board.SpawnRandomTile();

            Assert.IsFalse(spawned);
        }

        [Test]
        public void SpawnRandomTile_LandsInTheOnlyRemainingEmptyCell()
        {
            var board = new Board(new ScriptedRandomSource(0, 0));
            for (var y = 0; y < Board.Size; y++)
            for (var x = 0; x < Board.Size; x++)
                if (!(x == 2 && y == 1))
                    board.Seed(x, y, 2);

            board.SpawnRandomTile();

            Assert.IsNotNull(board.GetTile(2, 1));
        }
    }
}
