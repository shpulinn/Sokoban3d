using System.Collections.Generic;
using System.Threading;
using _Project.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LevelView : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _floorPrefab;
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _targetPrefab;
    [SerializeField] private PlayerView _playerPrefab;
    [SerializeField] private BoxView _boxPrefab;

    [Header("Settings")]
    [SerializeField] private float _cellSize = 1f;

    private PlayerView _playerView;
    private readonly List<BoxView> _boxViews = new();
    private LevelModel _model;

    public void Build(LevelModel model)
    {
        _model = model;

        // Очистить старые объекты
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        _boxViews.Clear();

        for (int x = 0; x < model.Width; x++)
        {
            for (int y = 0; y < model.Height; y++)
            {
                var pos = new GridPosition(x, y);
                var worldPos = GridToWorld(pos);
                var cell = model.GetCell(pos);

                if (cell == CellType.Wall)
                {
                    Instantiate(_wallPrefab, worldPos, Quaternion.identity, transform);
                    continue;
                }

                // Пол под всем кроме стен
                Instantiate(_floorPrefab, worldPos, Quaternion.identity, transform);

                if (cell == CellType.Target || cell == CellType.BoxOnTarget 
                    || cell == CellType.PlayerOnTarget)
                    Instantiate(_targetPrefab, worldPos + Vector3.up * 0.01f, 
                        Quaternion.identity, transform);

                if (cell == CellType.Player || cell == CellType.PlayerOnTarget)
                {
                    _playerView = Instantiate(_playerPrefab, worldPos, 
                        Quaternion.identity, transform);
                }

                if (cell == CellType.Box || cell == CellType.BoxOnTarget)
                {
                    var box = Instantiate(_boxPrefab, worldPos, 
                        Quaternion.identity, transform);
                    box.Init(pos);
                    box.SetOnTarget(cell == CellType.BoxOnTarget);
                    _boxViews.Add(box);
                }
            }
        }

        CenterCamera();
    }

    public async UniTask AnimateMove(
        GridPosition playerNewPos,
        GridPosition? boxOldPos,
        GridPosition? boxNewPos,
        CancellationToken ct)
    {
        var tasks = new List<UniTask>();

        tasks.Add(_playerView.MoveTo(GridToWorld(playerNewPos), ct));

        if (boxOldPos.HasValue && boxNewPos.HasValue)
        {
            var boxView = _boxViews.Find(b => b.GridPos == boxOldPos.Value);
            if (boxView != null)
            {
                bool onTarget = _model.GetCell(boxNewPos.Value) == CellType.BoxOnTarget;
                tasks.Add(boxView.MoveTo(GridToWorld(boxNewPos.Value), boxNewPos.Value, ct));
                // Обновить материал после анимации
                boxView.SetOnTarget(onTarget);
            }
        }

        await UniTask.WhenAll(tasks);
    }

    public Vector3 GridToWorld(GridPosition pos)
        => new Vector3(pos.X * _cellSize, 0f, pos.Y * _cellSize);

    private void CenterCamera()
    {
        if (Camera.main == null) return;
        float cx = (_model.Width - 1) * _cellSize / 2f;
        float cy = (_model.Height - 1) * _cellSize / 2f;
        Camera.main.transform.position = new Vector3(cx, 8f, cy - 2f);
        Camera.main.transform.rotation = Quaternion.Euler(55f, 0f, 0f);
    }
}