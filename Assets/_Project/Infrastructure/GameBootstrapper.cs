using _Project.Core.Interfaces;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private LevelController _levelController;
    [SerializeField] private LevelRepositoryBase _levelRepository;

    private void Awake()
    {
        var saveService = new SaveService(_levelRepository.GetLevelCount());
        _levelController.Init(saveService);
    }
}