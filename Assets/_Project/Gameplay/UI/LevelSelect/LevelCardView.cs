using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCardView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelNumberText;
    [SerializeField] private TextMeshProUGUI _bestMovesText;
    [SerializeField] private GameObject _lockIcon;
    [SerializeField] private GameObject _completedIcon;
    [SerializeField] private Button _button;
    
    public void Setup(int levelIndex, LevelProgress progress, System.Action onSelected)
    {
        _levelNumberText.text = $"{levelIndex + 1}";
        _button.onClick.RemoveAllListeners();

        if (!progress.IsUnlocked)
        {
            _lockIcon.SetActive(true);
            _completedIcon.SetActive(false);
            _bestMovesText.gameObject.SetActive(false);
            _button.interactable = false;
            return;
        }

        _lockIcon.SetActive(false);
        _button.interactable = true;
        _button.onClick.AddListener(() => onSelected?.Invoke());

        if (progress.IsCompleted)
        {
            _completedIcon.SetActive(true);
            _bestMovesText.gameObject.SetActive(true);
            _bestMovesText.text = $"Best: {progress.BestMoves}";
        }
        else
        {
            _completedIcon.SetActive(false);
            _bestMovesText.gameObject.SetActive(false);
        }
    }
}
