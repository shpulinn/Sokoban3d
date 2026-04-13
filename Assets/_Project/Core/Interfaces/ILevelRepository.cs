using _Project.Core;

public interface ILevelRepository
{
    LevelModel GetLevel(int index);
    int GetLevelCount();
}