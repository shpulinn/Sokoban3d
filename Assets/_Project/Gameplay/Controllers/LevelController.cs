using Cysharp.Threading.Tasks;
using System.Threading;
using _Project.Core;
using _Project.Core.Commands;
using _Project.Core.Interfaces;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelView _levelView;
    [SerializeField] private HUDView _hudView;
    
    [SerializeField] private LevelRepositoryBase _levelRepository;
    [SerializeField] private WinScreenView _winScreenView;

    private int _currentLevelIndex = 0;

    private LevelModel _model;
    private CommandHistory _history;
    private System.Action _onHistoryChanged;
    private CancellationTokenSource _cts;
    private bool _isAnimating;
    private SaveServiceBase _saveService;

    public void Init(SaveServiceBase saveService)
    {
        _saveService = saveService;
    }
    
    private void Start()
    {
        //_saveService = new SaveService(_levelRepository.GetLevelCount());
        LoadLevel();
    }

    private void LoadLevel()
    {
        _isAnimating = false;
        _winScreenView.Hide();
        
        _model = _levelRepository.GetLevel(_currentLevelIndex);

        if (_history != null && _onHistoryChanged != null)
            _history.OnHistoryChanged -= _onHistoryChanged;
        
        _history = new CommandHistory();
        _levelView.Build(_model);
        
        _hudView.Init(_history, TryUndo);
        _hudView.SetRestartAction(Restart);
        _hudView.UpdateUndoButton(false);

        // Обновлять кнопку Undo при каждом изменении истории
        _onHistoryChanged = () => _hudView.UpdateUndoButton(_history.CanUndo);
        _history.OnHistoryChanged += _onHistoryChanged;
    }

    private void Update()
    {
        if (_isAnimating) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            TryMove(GridPosition.Up);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            TryMove(GridPosition.Down);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            TryMove(GridPosition.Left);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            TryMove(GridPosition.Right);

        if (Input.GetKeyDown(KeyCode.Z) &&
            (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            TryUndo();

        if (Input.GetKeyDown(KeyCode.R))
            Restart();
    }

    private void TryMove(GridPosition delta)
    {
        if (!_model.CanMove(delta)) return;
        ExecuteMoveAsync(delta).Forget();
    }

    private async UniTaskVoid ExecuteMoveAsync(GridPosition delta)
    {
        _isAnimating = true;
        _hudView.SetInteractable(false);
        
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        bool pushesBox = _model.CanPushBox(delta);
        GridPosition? boxOldPos = pushesBox ? _model.PlayerPosition + delta : null;
        GridPosition? boxNewPos = pushesBox ? boxOldPos + delta : null;

        ICommand cmd = pushesBox
            ? new PushBoxCommand(_model, delta)
            : new MovePlayerCommand(_model, delta);

        _history.Execute(cmd);

        await _levelView.AnimateMove(
            _model.PlayerPosition, boxOldPos, boxNewPos, _cts.Token);

        _isAnimating = false;
        _hudView.SetInteractable(true);

        if (_model.IsCompleted())
            OnLevelComplete();
    }

    private void TryUndo()
    {
        if (!_history.CanUndo) return;
        
        // отменяем cts токен
        _cts?.Cancel();
        _cts?.Dispose();
        // создаем новый cts токен
        _cts = new CancellationTokenSource();
        _isAnimating = false;
        
        _history.Undo();
        _levelView.Build(_model);
        _hudView.SetInteractable(true);
    }

    private void Restart()
    {
        _cts?.Cancel();
        _isAnimating = false;
        LoadLevel();
    }
    
    private void OnLevelComplete()
    {
        _isAnimating = true;
        _hudView.SetInteractable(false);

        _saveService.CompleteLevel(_currentLevelIndex, _history.MoveCount);

        bool hasNext = _currentLevelIndex + 1 < _levelRepository.GetLevelCount();

        _winScreenView.Show(
            _history.MoveCount,
            hasNext,
            onNext: () =>
            {
                _currentLevelIndex++;
                LoadLevel();
            },
            onRestart: Restart
        );
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}