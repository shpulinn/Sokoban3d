using _Project.Core.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private LevelSelectView _levelSelectView;
    [SerializeField] private LevelRepositoryBase _levelRepository;

    private SaveServiceBase _saveService;

    private void Start()
    {
        _saveService = new SaveService(_levelRepository.GetLevelCount());
        BuildLevelSelect();
    }

    private void BuildLevelSelect()
    {
        _levelSelectView.Build(
            _levelRepository.GetLevelCount(),
            _saveService,
            onLevelSelected: LoadLevel
        );
    }

    private void LoadLevel(int levelIndex)
    {
        SceneData.SelectedLevelIndex = levelIndex;
        SceneManager.LoadScene("Game");
    }
}