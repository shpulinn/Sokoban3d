using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private float _moveDuration = 0.15f;

    public UniTask MoveTo(Vector3 targetPos, CancellationToken ct)
    {
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
}