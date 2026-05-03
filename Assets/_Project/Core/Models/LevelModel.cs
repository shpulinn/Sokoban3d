using System;

namespace _Project.Core
{
    public class LevelModel
    {
        private readonly CellType[,] _grid;

        public int Width { get; }
        public int Height { get; }
        public GridPosition PlayerPosition { get; private set; }

        public event Action OnStateChanged;
        public event Action OnBoxPlacedOnTarget;

        public LevelModel(int width, int height, CellType[,] grid, GridPosition playerStart)
        {
            Width = width;
            Height = height;
            _grid = (CellType[,])grid.Clone();
            PlayerPosition = playerStart;
        }

        public CellType GetCell(GridPosition pos) => _grid[pos.X, pos.Y];

        private void SetCell(GridPosition pos, CellType type)
        {
            _grid[pos.X, pos.Y] = type;
        }

        public bool IsWalkable(GridPosition pos)
        {
            if (pos.X < 0 || pos.X >= Width || pos.Y < 0 || pos.Y >= Height) return false;
            var cell = GetCell(pos);
            return cell == CellType.Floor || cell == CellType.Target;
        }

        public bool HasBox(GridPosition pos)
        {
            var cell = GetCell(pos);
            return cell == CellType.Box || cell == CellType.BoxOnTarget;
        }

        public bool CanMove(GridPosition delta)
        {
            var next = PlayerPosition + delta;
            if (!IsInBounds(next)) return false;
            if (GetCell(next) == CellType.Wall) return false;
            if (HasBox(next))
            {
                var afterBox = next + delta;
                return IsInBounds(afterBox) && IsWalkable(afterBox);
            }
            return IsWalkable(next);
        }

        public bool CanPushBox(GridPosition delta)
        {
            var next = PlayerPosition + delta;
            return IsInBounds(next) && HasBox(next);
        }

        // Вызывается из команд напрямую
        public void MovePlayer(GridPosition delta)
        {
            var next = PlayerPosition + delta;

            // Снять игрока с текущей клетки
            var currentCell = GetCell(PlayerPosition);
            SetCell(PlayerPosition, currentCell == CellType.PlayerOnTarget
                ? CellType.Target
                : CellType.Floor);

            // Поставить игрока на новую клетку
            var nextCell = GetCell(next);
            SetCell(next, nextCell == CellType.Target
                ? CellType.PlayerOnTarget
                : CellType.Player);

            PlayerPosition = next;
            OnStateChanged?.Invoke();
        }

        public void MoveBox(GridPosition from, GridPosition to)
        {
            var fromCell = GetCell(from);
            SetCell(from, fromCell == CellType.BoxOnTarget ? CellType.Target : CellType.Floor);

            var toCell = GetCell(to);
            SetCell(to, toCell == CellType.Target ? CellType.BoxOnTarget : CellType.Box);
            
            if (toCell == CellType.Target)
                OnBoxPlacedOnTarget?.Invoke();

            OnStateChanged?.Invoke();
        }

        public bool IsCompleted()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (_grid[x, y] == CellType.Target || _grid[x, y] == CellType.PlayerOnTarget)
                        return false; // есть незакрытая цель
            return true;
        }

        private bool IsInBounds(GridPosition pos)
            => pos.X >= 0 && pos.X < Width && pos.Y >= 0 && pos.Y < Height;
    }
}