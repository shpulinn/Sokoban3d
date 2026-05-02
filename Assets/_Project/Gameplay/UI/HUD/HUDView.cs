using _Project.Core.Commands;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _movesText;
    [SerializeField] private Button _undoButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _menuButton;
    
    private System.Action _onHistoryChanged;
    private CommandHistory _currentHistory;

    public void Init(CommandHistory history, System.Action onUndo)
    {
        if (_onHistoryChanged != null && _currentHistory != null)
            _currentHistory.OnHistoryChanged -= _onHistoryChanged;
        
        _currentHistory = history;
        _onHistoryChanged = () => UpdateMoves(history.MoveCount);
        _currentHistory.OnHistoryChanged += _onHistoryChanged;

        _undoButton.onClick.RemoveAllListeners();
        _undoButton.onClick.AddListener(() => onUndo?.Invoke());
        _restartButton.onClick.RemoveAllListeners();
        
        _menuButton.onClick.RemoveAllListeners();
        _menuButton.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene("Menu"));
        
        UpdateMoves(0);
    }

    public void SetRestartAction(System.Action onRestart)
    {
        _restartButton.onClick.AddListener(() => onRestart?.Invoke());
    }

    public void SetInteractable(bool interactable)
    {
        _undoButton.interactable = interactable && true;
        _restartButton.interactable = interactable;
    }

    public void UpdateUndoButton(bool canUndo)
    {
        _undoButton.interactable = canUndo;
    }

    private void UpdateMoves(int count)
    {
        _movesText.text = $"Moves: {count}";
    }

    private void OnDestroy()
    {
        _undoButton.onClick.RemoveAllListeners();
        _restartButton.onClick.RemoveAllListeners();
    }
}