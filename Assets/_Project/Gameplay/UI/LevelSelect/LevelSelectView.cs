using _Project.Core.Interfaces;
using UnityEngine;

public class LevelSelectView : MonoBehaviour
{
    [SerializeField] private LevelCardView _cardPrefab;
    [SerializeField] private Transform _grid;

    public void Build(int levelCount, SaveServiceBase saveService, 
        System.Action<int> onLevelSelected)
    {
        foreach (Transform child in _grid)
            Destroy(child.gameObject);

        for (int i = 0; i < levelCount; i++)
        {
            int index = i;
            var card = Instantiate(_cardPrefab, _grid);
            var progress = saveService.GetProgress(index);
            card.Setup(index, progress, () => onLevelSelected?.Invoke(index));
        }
    }
}