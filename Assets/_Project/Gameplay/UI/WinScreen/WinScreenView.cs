using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinScreenView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _movesText;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private CanvasGroup _canvasGroup;
    
    public void Show(int moves, bool hasNextLevel,
        System.Action onNext, System.Action onRestart)
    {
        _movesText.text = $"Moves: {moves}";
        _nextLevelButton.gameObject.SetActive(hasNextLevel);

        _nextLevelButton.onClick.RemoveAllListeners();
        _restartButton.onClick.RemoveAllListeners();
        _nextLevelButton.onClick.AddListener(() => onNext?.Invoke());
        _restartButton.onClick.AddListener(() => onRestart?.Invoke());

        gameObject.SetActive(true);
        _canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.8f;

        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        AnimateFadeIn().Forget();
    }

    public void Hide()
    {
        AnimateFadeOut().Forget();
    }
    
    private async UniTaskVoid AnimateFadeIn()
    {
        float duration = 0.3f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            await UniTask.Yield();
        }
        _canvasGroup.alpha = 1f;
    }

    private async UniTaskVoid AnimateFadeOut()
    {
        float duration = 0.2f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / duration);
            await UniTask.Yield();
        }
        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
