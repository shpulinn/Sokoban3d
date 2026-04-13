namespace _Project.Core.Commands
{
    public class PushBoxCommand : ICommand
    {
        private readonly LevelModel _model;
        private readonly GridPosition _delta;
        private GridPosition _playerPrev;
        private GridPosition _boxFrom;
        private GridPosition _boxTo;
        
        public PushBoxCommand(LevelModel model, GridPosition delta)
        {
            _model = model;
            _delta = delta;
        }
        
        public void Execute()
        {
            _playerPrev = _model.PlayerPosition;
            _boxFrom    = _playerPrev + _delta;
            _boxTo      = _boxFrom + _delta;

            _model.MoveBox(_boxFrom, _boxTo);
            _model.MovePlayer(_delta);
        }

        public void Undo()
        {
            // Порядок важен: сначала вернуть игрока, потом ящик
            var backDelta = new GridPosition(
                _playerPrev.X - _model.PlayerPosition.X,
                _playerPrev.Y - _model.PlayerPosition.Y);

            _model.MovePlayer(backDelta);
            _model.MoveBox(_boxTo, _boxFrom);
        }
    }
}