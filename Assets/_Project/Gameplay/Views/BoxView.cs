using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using _Project.Core;
using UnityEngine;

public class BoxView : MonoBehaviour
{
    [SerializeField] private float _moveDuration = 0.15f;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Material _normalMaterial;
    [SerializeField] private Material _onTargetMaterial;

    public GridPosition GridPos { get; private set; }

    public void Init(GridPosition pos) => GridPos = pos;

    public UniTask MoveTo(Vector3 targetPos, GridPosition newGridPos, CancellationToken ct)
    {
        GridPos = newGridPos;
        var utcs = new UniTaskCompletionSource();

        transform
            .DOMove(targetPos, _moveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => utcs.TrySetResult())
            .OnKill(() => utcs.TrySetResult());

        ct.Register(() =>
        {
            if (this != null)
                transform.DOKill();
            utcs.TrySetResult();
        });

        return utcs.Task;
    }

    public void SetOnTarget(bool onTarget)
    {
        if (_renderer == null) return;
        _renderer.material = onTarget ? _onTargetMaterial : _normalMaterial;
    }
}