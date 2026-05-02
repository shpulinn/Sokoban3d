namespace _Project.Core.Interfaces
{
    public abstract class SaveServiceBase
    {
        public abstract LevelProgress GetProgress(int levelIndex);
        public abstract void CompleteLevel(int levelIndex, int moves);
        public abstract void UnlockLevel(int levelIndex);
        public abstract void ResetAll(int levelCount);
    }
}