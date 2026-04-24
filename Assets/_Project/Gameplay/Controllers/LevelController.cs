using Cysharp.Threading.Tasks;
using System.Threading;
using _Project.Core;
using _Project.Core.Commands;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelData _levelData;
    [SerializeField] private LevelView _levelView;

    private LevelModel _model;
    private CommandHistory _history;
    private CancellationTokenSource _cts;
    private bool _isAnimating;

    private void Start()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        _model = _levelData.ToLevelModel();
        _history = new CommandHistory();
        _levelView.Build(_model);
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
        _cts?.Cancel();
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

        if (_model.IsCompleted())
            OnLevelComplete();
    }

    private void TryUndo()
    {
        if (!_history.CanUndo) return;
        _history.Undo();
        // После undo — полностью перестраиваем View по актуальной модели
        _levelView.Build(_model);
    }

    private void Restart()
    {
        _cts?.Cancel();
        _isAnimating = false;
        LoadLevel();
    }

    private void OnLevelComplete()
    {
        Debug.Log($"[LevelController] LEVEL COMPLETE! Moves: {_history.MoveCount}");
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}