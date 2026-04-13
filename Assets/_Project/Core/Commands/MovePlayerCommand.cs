namespace _Project.Core.Commands
{
    public class MovePlayerCommand : ICommand
    {
        private readonly LevelModel _model;
        private readonly GridPosition _delta;
        private GridPosition _prevPosition;

        public MovePlayerCommand(LevelModel model, GridPosition delta)
        {
            _model = model;
            _delta = delta;
        }

        public void Execute()
        {
            _prevPosition = _model.PlayerPosition;
            _model.MovePlayer(_delta);
        }

        public void Undo()
        {
            var backDelta = new GridPosition(
                _prevPosition.X - _model.PlayerPosition.X,
                _prevPosition.Y - _model.PlayerPosition.Y);
            _model.MovePlayer(backDelta);
        }
    }
}