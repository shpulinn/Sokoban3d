using _Project.Core;
using UnityEngine;

namespace _Project.Data
{
    [CreateAssetMenu(fileName = "Level_01", menuName = "Sokoban/Level Data")]
    public class LevelData : ScriptableObject
    {
        public string LevelName;
        public int Width;
        public int Height;
        public CellType[] Cells; // row-major: index = Y * Width + X

        public CellType GetCell(int x, int y) => Cells[y * Width + x];

        public GridPosition FindPlayer()
        {
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                if (GetCell(x, y) == CellType.Player || 
                    GetCell(x, y) == CellType.PlayerOnTarget)
                    return new GridPosition(x, y);

            throw new System.Exception($"[LevelData] No player found in {name}");
        }

        public LevelModel ToLevelModel()
        {
            var grid = new CellType[Width, Height];
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                grid[x, y] = GetCell(x, y);

            return new LevelModel(Width, Height, grid, FindPlayer());
        }
    }
}