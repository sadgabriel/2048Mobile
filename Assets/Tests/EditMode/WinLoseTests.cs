using Game2048.Model;
using NUnit.Framework;

namespace Game2048.Model.Tests
{
    public class WinLoseTests
    {
        [Test]
        public void HasWon_FalseInitially()
        {
            var board = new Board();
            board.Seed(0, 0, 4);

            Assert.IsFalse(board.HasWon);
        }

        [Test]
        public void HasWon_BecomesTrueWhenMergeCreates2048Tile()
        {
            var board = new Board();
            board.Seed(0, 0, 1024);
            board.Seed(1, 0, 1024);

            board.Move(Direction.Left);

            Assert.IsTrue(board.HasWon);
            Assert.AreEqual(2048, board.GetTile(0, 0).Value);
        }

        [Test]
        public void HasWon_StaysTrueAfterFurtherMoves()
        {
            var board = new Board();
            board.Seed(0, 0, 1024);
            board.Seed(1, 0, 1024);
            board.Move(Direction.Left);

            board.Seed(3, 0, 2);
            board.Move(Direction.Right);

            Assert.IsTrue(board.HasWon);
        }

        [Test]
        public void IsGameOver_FalseWhenEmptyCellsExist()
        {
            var board = new Board();
            board.Seed(0, 0, 2);

            Assert.IsFalse(board.IsGameOver());
        }

        [Test]
        public void IsGameOver_FalseWhenFullBoardHasAnAdjacentEqualPair()
        {
            var board = new Board();
            var values = new[]
            {
                2, 4, 2, 4,
                4, 2, 4, 2,
                2, 4, 2, 4,
                4, 2, 4, 4
            };
            for (var y = 0; y < Board.Size; y++)
            for (var x = 0; x < Board.Size; x++)
                board.Seed(x, y, values[y * Board.Size + x]);

            Assert.IsFalse(board.IsGameOver());
        }

        [Test]
        public void IsGameOver_TrueWhenFullBoardHasNoAdjacentEqualPair()
        {
            var board = new Board();
            var values = new[]
            {
                2, 4, 2, 4,
                4, 2, 4, 2,
                2, 4, 2, 4,
                4, 2, 4, 2
            };
            for (var y = 0; y < Board.Size; y++)
            for (var x = 0; x < Board.Size; x++)
                board.Seed(x, y, values[y * Board.Size + x]);

            Assert.IsTrue(board.IsGameOver());
        }

        [Test]
        public void IsGameOver_DoesNotTreatEndOfRowAndStartOfNextRowAsAdjacent()
        {
            var board = new Board();
            // (3,0) and (0,1) both hold 2 — adjacent in a flat row-major array
            // (indices 3 and 4) but not adjacent on the actual grid. A row-boundary
            // bug here would wrongly report a live move and this test would fail.
            var values = new[]
            {
                4, 8, 4, 2,
                2, 4, 8, 4,
                4, 2, 4, 8,
                8, 4, 2, 4
            };
            for (var y = 0; y < Board.Size; y++)
            for (var x = 0; x < Board.Size; x++)
                board.Seed(x, y, values[y * Board.Size + x]);

            Assert.IsTrue(board.IsGameOver());
        }
    }
}
