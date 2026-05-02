using System;
using System.IO;
using _Project.Core.Interfaces;
using UnityEngine;

public class SaveService : SaveServiceBase
{
    private readonly string _savePath;
    private SaveData _data;

    public SaveService(int levelCount)
    {
        _savePath = Path.Combine(Application.persistentDataPath, "save.json");
        Load(levelCount);
    }

    public override LevelProgress GetProgress(int levelIndex) => _data.Levels[levelIndex];

    public override  void UnlockLevel(int levelIndex)
    {
        if (levelIndex >= _data.Levels.Length) return;
        _data.Levels[levelIndex].IsUnlocked = true;
        Save();
    }

    public override  void CompleteLevel(int levelIndex, int moves)
    {
        var progress = _data.Levels[levelIndex];
        progress.IsCompleted = true;
        progress.BestMoves = progress.BestMoves == 0
            ? moves
            : Mathf.Min(progress.BestMoves, moves);
        
        if (levelIndex + 1 < _data.Levels.Length)
            _data.Levels[levelIndex + 1].IsUnlocked = true;

        Save();
    }

    public override  void ResetAll(int levelCount)
    {
        InitData(levelCount);
        Save();
    }

    private void Load(int levelCount)
    {
        if (File.Exists(_savePath))
        {
            try
            {
                var json = File.ReadAllText(_savePath);
                _data = JsonUtility.FromJson<SaveData>(json);
                
                if (_data.Levels.Length < levelCount)
                {
                    var old = _data.Levels;
                    _data.Levels = new LevelProgress[levelCount];
                    Array.Copy(old, _data.Levels, old.Length);
                    for (int i = old.Length; i < levelCount; i++)
                        _data.Levels[i] = new LevelProgress();
                }
                return;
            }
            catch
            {
                Debug.LogWarning("[SaveService] Save file corrupted, resetting.");
            }
        }

        InitData(levelCount);
    }

    private void InitData(int levelCount)
    {
        _data = new SaveData { Levels = new LevelProgress[levelCount] };
        for (int i = 0; i < levelCount; i++)
            _data.Levels[i] = new LevelProgress { IsUnlocked = i == 0 };
        Save();
    }

    private void Save()
    {
        var json = JsonUtility.ToJson(_data, prettyPrint: true);
        File.WriteAllText(_savePath, json);
    }
}