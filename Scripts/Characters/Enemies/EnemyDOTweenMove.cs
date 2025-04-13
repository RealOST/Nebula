using UnityEngine;
using DG.Tweening;

public class EnemyDOTweenMove : MonoBehaviour {
    public Transform[] pathPoints;

    private void Start() {
        var positions = new Vector3[pathPoints.Length];
        for (var i = 0; i < pathPoints.Length; i++)
            positions[i] = pathPoints[i].position;

        transform.DOPath(positions, 5f, PathType.Linear, PathMode.Sidescroller2D, 10, Color.yellow)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }
}