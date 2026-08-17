using Game2048.Model;
using NUnit.Framework;

namespace Game2048.Model.Tests
{
    public class MergeLogicTests
    {
        [Test]
        public void TwoEqualTiles_MergeIntoDoubleValue_AndAddScore()
        {
            var board = new Board();
            board.Seed(0, 0, 2);
            board.Seed(1, 0, 2);

            var result = board.Move(Direction.Left);

            Assert.AreEqual(4, board.GetTile(0, 0).Value);
            Assert.IsNull(board.GetTile(1, 0));
            Assert.IsTrue(result.Moved);
            Assert.AreEqual(4, result.ScoreGained);
            Assert.AreEqual(4, board.Score);
        }

        [Test]
        public void ThreeEqualTiles_MergeOnlyOncePerPair()
        {
            var board = new Board();
            board.Seed(0, 0, 2);
            board.Seed(1, 0, 2);
            board.Seed(2, 0, 2);

            board.Move(Direction.Left);

            Assert.AreEqual(4, board.GetTile(0, 0).Value);
            Assert.AreEqual(2, board.GetTile(1, 0).Value);
            Assert.IsNull(board.GetTile(2, 0));
        }

        [Test]
        public void FourEqualTiles_MergeIntoTwoPairs()
        {
            var board = new Board();
            board.Seed(0, 0, 2);
            board.Seed(1, 0, 2);
            board.Seed(2, 0, 2);
            board.Seed(3, 0, 2);

            var result = board.Move(Direction.Left);

            Assert.AreEqual(4, board.GetTile(0, 0).Value);
            Assert.AreEqual(4, board.GetTile(1, 0).Value);
            Assert.IsNull(board.GetTile(2, 0));
            Assert.IsNull(board.GetTile(3, 0));
            Assert.AreEqual(8, result.ScoreGained);
        }

        [Test]
        public void MergedTile_DoesNotReMergeWithNextEqualTileInSameMove()
        {
            var board = new Board();
            board.Seed(0, 0, 4);
            board.Seed(1, 0, 2);
            board.Seed(2, 0, 2);
            board.Seed(3, 0, 4);

            board.Move(Direction.Left);

            Assert.AreEqual(4, board.GetTile(0, 0).Value);
            Assert.AreEqual(4, board.GetTile(1, 0).Value);
            Assert.AreEqual(4, board.GetTile(2, 0).Value);
            Assert.IsNull(board.GetTile(3, 0));
        }

        [Test]
        public void Score_AccumulatesAcrossMultipleLinesInOneMove()
        {
            var board = new Board();
            board.Seed(0, 0, 2);
            board.Seed(1, 0, 2);
            board.Seed(0, 1, 8);
            board.Seed(1, 1, 8);

            var result = board.Move(Direction.Left);

            Assert.AreEqual(20, result.ScoreGained);
            Assert.AreEqual(20, board.Score);
        }

        [Test]
        public void Merge_WorksWhenSwipingRight()
        {
            var board = new Board();
            board.Seed(2, 0, 2);
            board.Seed(3, 0, 2);

            board.Move(Direction.Right);

            Assert.AreEqual(4, board.GetTile(3, 0).Value);
            Assert.IsNull(board.GetTile(2, 0));
        }

        [Test]
        public void Merge_WorksWhenSwipingUp()
        {
            var board = new Board();
            board.Seed(0, 0, 2);
            board.Seed(0, 1, 2);

            board.Move(Direction.Up);

            Assert.AreEqual(4, board.GetTile(0, 0).Value);
            Assert.IsNull(board.GetTile(0, 1));
        }

        [Test]
        public void Merge_WorksWhenSwipingDown()
        {
            var board = new Board();
            board.Seed(0, 2, 2);
            board.Seed(0, 3, 2);

            board.Move(Direction.Down);

            Assert.AreEqual(4, board.GetTile(0, 3).Value);
            Assert.IsNull(board.GetTile(0, 2));
        }
    }
}
