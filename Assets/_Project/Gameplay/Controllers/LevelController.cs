using _Project.Core;
using _Project.Core.Commands;
using _Project.Data;
using UnityEngine;

namespace _Project.Gameplay.Controllers
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private LevelData _levelData;

        private LevelModel _model;
        private CommandHistory _history;

        private void Start()
        {
            _model = _levelData.ToLevelModel();
            _history = new CommandHistory();
            Debug.Log($"[LevelController] Level loaded: {_levelData.LevelName}, " +
                      $"Player at {_model.PlayerPosition}");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                TryMove(GridPosition.Up);
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                TryMove(GridPosition.Down);
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                TryMove(GridPosition.Left);
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                TryMove(GridPosition.Right);

            if (Input.GetKeyDown(KeyCode.Z) && (Input.GetKey(KeyCode.LeftControl) 
                                                || Input.GetKey(KeyCode.RightControl)))
                _history.Undo();

            if (Input.GetKeyDown(KeyCode.R))
                Restart();
        }

        private void TryMove(GridPosition delta)
        {
            if (!_model.CanMove(delta)) return;

            ICommand cmd = _model.CanPushBox(delta)
                ? new PushBoxCommand(_model, delta)
                : new MovePlayerCommand(_model, delta);

            _history.Execute(cmd);

            Debug.Log($"[LevelController] Move: Player → {_model.PlayerPosition}, " +
                      $"Moves: {_history.MoveCount}");

            if (_model.IsCompleted())
                Debug.Log("[LevelController] LEVEL COMPLETE!");
        }

        private void Restart()
        {
            _model = _levelData.ToLevelModel();
            _history.Clear();
            Debug.Log("[LevelController] Restarted");
        }
    }
}