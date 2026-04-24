using UnityEngine;

namespace _Project.Core.Interfaces
{
    public abstract class LevelRepositoryBase : MonoBehaviour
    {
        public abstract LevelModel GetLevel(int index);
        public abstract int GetLevelCount();
    }
}