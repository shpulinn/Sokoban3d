using _Project.Core;
using _Project.Core.Commands;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace Tests.EditMode
{
    public class LevelModelTests
    {
        // Модель 3×1: [Floor, Player, Box, Target, Floor]
        private LevelModel BuildLinearLevel()
        {
            // Уровень 5×1: W P B T W
            var grid = new CellType[5, 1];
            grid[0, 0] = CellType.Wall;
            grid[1, 0] = CellType.Player;
            grid[2, 0] = CellType.Box;
            grid[3, 0] = CellType.Target;
            grid[4, 0] = CellType.Wall;
            return new LevelModel(5, 1, grid, new GridPosition(1, 0));
        }

        [Test]
        public void Player_CannotMoveIntoWall()
        {
            var model = BuildLinearLevel();
            Assert.IsFalse(model.CanMove(GridPosition.Left));
        }

        [Test]
        public void Player_CanPushBox()
        {
            var model = BuildLinearLevel();
            Assert.IsTrue(model.CanMove(GridPosition.Right));
        }

        [Test]
        public void Level_CompletesWhenBoxOnTarget()
        {
            var model = BuildLinearLevel();
            var history = new CommandHistory();
            history.Execute(new PushBoxCommand(model, GridPosition.Right));
            Assert.IsTrue(model.IsCompleted());
        }

        [Test]
        public void Undo_RestoresPreviousState()
        {
            var model = BuildLinearLevel();
            var history = new CommandHistory();
            var startPos = model.PlayerPosition;

            history.Execute(new PushBoxCommand(model, GridPosition.Right));
            history.Undo();

            Assert.AreEqual(startPos, model.PlayerPosition);
            Assert.AreEqual(CellType.Box, model.GetCell(new GridPosition(2, 0)));
        }
    }
}