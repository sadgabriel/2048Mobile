using Game2048.Model;
using NUnit.Framework;

namespace Game2048.Model.Tests
{
    public class BoardTests
    {
        [Test]
        public void NewBoard_AllCellsEmpty()
        {
            var board = new Board();

            for (var y = 0; y < Board.Size; y++)
            for (var x = 0; x < Board.Size; x++)
                Assert.IsNull(board.GetTile(x, y));
        }

        [Test]
        public void MoveLeft_CompactsTilesWithoutGaps()
        {
            var board = new Board();
            board.Seed(1, 0, 2);
            board.Seed(3, 0, 4);

            board.Move(Direction.Left);

            Assert.AreEqual(2, board.GetTile(0, 0).Value);
            Assert.AreEqual(4, board.GetTile(1, 0).Value);
            Assert.IsNull(board.GetTile(2, 0));
            Assert.IsNull(board.GetTile(3, 0));
        }

        [Test]
        public void MoveRight_CompactsTilesWithoutGaps()
        {
            var board = new Board();
            board.Seed(0, 0, 2);
            board.Seed(2, 0, 4);

            board.Move(Direction.Right);

            Assert.AreEqual(2, board.GetTile(2, 0).Value);
            Assert.AreEqual(4, board.GetTile(3, 0).Value);
            Assert.IsNull(board.GetTile(0, 0));
            Assert.IsNull(board.GetTile(1, 0));
        }

        [Test]
        public void MoveUp_CompactsTilesWithoutGaps()
        {
            var board = new Board();
            board.Seed(0, 1, 2);
            board.Seed(0, 3, 4);

            board.Move(Direction.Up);

            Assert.AreEqual(2, board.GetTile(0, 0).Value);
            Assert.AreEqual(4, board.GetTile(0, 1).Value);
            Assert.IsNull(board.GetTile(0, 2));
            Assert.IsNull(board.GetTile(0, 3));
        }

        [Test]
        public void MoveDown_CompactsTilesWithoutGaps()
        {
            var board = new Board();
            board.Seed(0, 0, 2);
            board.Seed(0, 2, 4);

            board.Move(Direction.Down);

            Assert.AreEqual(2, board.GetTile(0, 2).Value);
            Assert.AreEqual(4, board.GetTile(0, 3).Value);
            Assert.IsNull(board.GetTile(0, 0));
            Assert.IsNull(board.GetTile(0, 1));
        }

        [Test]
        public void Move_TileAlreadyAtDestinationEdge_ReturnsNotMoved()
        {
            var board = new Board();
            board.Seed(0, 0, 2);

            var result = board.Move(Direction.Left);

            Assert.IsFalse(result.Moved);
        }

        [Test]
        public void Move_DifferentValuesAlreadyCompacted_ReturnsNotMoved()
        {
            var board = new Board();
            board.Seed(0, 0, 2);
            board.Seed(1, 0, 4);

            var result = board.Move(Direction.Left);

            Assert.IsFalse(result.Moved);
        }

        [Test]
        public void Move_TileSlidesIntoGap_ReturnsMoved()
        {
            var board = new Board();
            board.Seed(2, 0, 2);

            var result = board.Move(Direction.Left);

            Assert.IsTrue(result.Moved);
        }
    }
}
