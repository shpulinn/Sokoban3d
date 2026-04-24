using _Project.Core;
using _Project.Core.Interfaces;
using UnityEngine;

public class LevelRepository : LevelRepositoryBase
{
    [SerializeField] private LevelData[] _levels;

    public override LevelModel GetLevel(int index) => _levels[index].ToLevelModel();
    public override int GetLevelCount() => _levels.Length;
}