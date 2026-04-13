using _Project.Core;
using _Project.Data;
using UnityEngine;

public class LevelRepository : MonoBehaviour, ILevelRepository
{
    [SerializeField] private LevelData[] _levels;

    public LevelModel GetLevel(int index) => _levels[index].ToLevelModel();
    public int GetLevelCount() => _levels.Length;
}