using System.Linq;
using Game2048.Model;
using NUnit.Framework;

namespace Game2048.Model.Tests
{
    public class TileMovementTests
    {
        [Test]
        public void SimpleSlide_ProducesOneMovement_NotConsumedByMerge()
        {
            var board = new Board();
            board.Seed(2, 0, 2);
            var tileId = board.GetTile(2, 0).Id;

            board.Move(Direction.Left, out var movements);

            Assert.AreEqual(1, movements.Count);
            var movement = movements[0];
            Assert.AreEqual(tileId, movement.TileId);
            Assert.AreEqual(2, movement.FromX);
            Assert.AreEqual(0, movement.FromY);
            Assert.AreEqual(0, movement.ToX);
            Assert.AreEqual(0, movement.ToY);
            Assert.IsFalse(movement.ConsumedByMerge);
        }

        [Test]
        public void NoChange_MovementStillReportsSamePosition()
        {
            var board = new Board();
            board.Seed(0, 0, 2);
            var tileId = board.GetTile(0, 0).Id;

            board.Move(Direction.Left, out var movements);

            Assert.AreEqual(1, movements.Count);
            Assert.AreEqual(tileId, movements[0].TileId);
            Assert.AreEqual(movements[0].FromX, movements[0].ToX);
            Assert.AreEqual(movements[0].FromY, movements[0].ToY);
        }

        [Test]
        public void Merge_ProducesTwoConsumedMovementsToSameDestination()
        {
            var board = new Board();
            board.Seed(0, 0, 2);
            board.Seed(1, 0, 2);
            var idA = board.GetTile(0, 0).Id;
            var idB = board.GetTile(1, 0).Id;

            board.Move(Direction.Left, out var movements);

            Assert.AreEqual(2, movements.Count);
            Assert.IsTrue(movements.All(m => m.ConsumedByMerge));
            Assert.IsTrue(movements.All(m => m.ToX == 0 && m.ToY == 0));
            CollectionAssert.AreEquivalent(new[] { idA, idB }, movements.Select(m => m.TileId).ToArray());
        }

        [Test]
        public void FourEqualTiles_ProducesTwoSeparateMergeDestinations()
        {
            var board = new Board();
            board.Seed(0, 0, 2);
            board.Seed(1, 0, 2);
            board.Seed(2, 0, 2);
            board.Seed(3, 0, 2);

            board.Move(Direction.Left, out var movements);

            Assert.AreEqual(4, movements.Count);
            Assert.IsTrue(movements.All(m => m.ConsumedByMerge));

            var destinations = movements.Select(m => (m.ToX, m.ToY)).Distinct().ToList();
            Assert.AreEqual(2, destinations.Count);
            CollectionAssert.Contains(destinations, (0, 0));
            CollectionAssert.Contains(destinations, (1, 0));
        }

        [Test]
        public void MoveWithoutOutParameter_StillBehavesTheSame()
        {
            var board = new Board();
            board.Seed(0, 0, 2);
            board.Seed(1, 0, 2);

            var result = board.Move(Direction.Left);

            Assert.IsTrue(result.Moved);
            Assert.AreEqual(4, result.ScoreGained);
        }
    }
}
