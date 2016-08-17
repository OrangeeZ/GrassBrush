using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class DistributedCircleGenerator : MonoBehaviour
{
    [Serializable]
    public class Circle
    {
        public Vector3 Position;
        public float Radius;
    }

    [SerializeField]
    private int _pointCount = 20;

    [SerializeField]
    private float _radius = 5f;

    [SerializeField]
    private float _minDistanceBetweenPoints = 0.5f;

    private List<Circle> _circles = new List<Circle>();

    public IList<Circle> GetCircles()
    {
        return _circles;
    }

    [ContextMenu("Generate seed")]
    public void Generate()
    {
        _circles.Clear();

        for (var i = 0; i < _pointCount; i++)
        {
            var point = Random.insideUnitCircle * _radius;
            var worldPoint = new Vector3(point.x, 0, point.y);

            _circles.Add(new Circle {Position = worldPoint + transform.position, Radius = float.NaN});
        }

        CalculateMinimumRadius();
        RemoveCirclesWithinMinDistance();
        CalculateMinimumRadius();
    }

    private void CalculateMinimumRadius()
    {
        for (var i = 0; i < _circles.Count; i++)
        {
            var minSqrDistance = float.MaxValue;
            var minDistanceCircle = default(Circle);

            for (var j = 0; j < _circles.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                var sqrDistance = (_circles[i].Position - _circles[j].Position).sqrMagnitude;

                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    minDistanceCircle = _circles[j];
                }
            }

            var minRadius = Mathf.Sqrt(minSqrDistance) * 0.5f;
            minDistanceCircle.Radius = Mathf.Min(minRadius, minDistanceCircle.Radius);
            _circles[i].Radius = minRadius;
        }
    }

    private void RemoveCirclesWithinMinDistance()
    {
        _circles.RemoveAll(_ => _.Radius < _minDistanceBetweenPoints);
    }

    void Update()
    {
        if (transform.hasChanged)
        {
            Generate();

            transform.hasChanged = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        for (var i = 0; i < _circles.Count; i++)
        {
            Gizmos.DrawSphere(_circles[i].Position, _circles[i].Radius);
        }
    }
}
