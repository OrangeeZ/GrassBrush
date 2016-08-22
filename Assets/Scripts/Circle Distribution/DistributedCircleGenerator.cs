using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class DistributedCircleGenerator
{
    [Serializable]
    public class Circle
    {
        public Vector3 Position;
        public float AngleY;
        public float Scale;

        public float Radius;

        public DetailPreset Prefab;

        public Color32 Color;

        [NonSerialized]
        public DetailPreset Instance;
    }

    [SerializeField]
    private int _pointsPerUnit = 1;

    [SerializeField]
    private float _radius = 5f;

    private List<Circle> _circles = new List<Circle>();

    public IList<Circle> GetCircles()
    {
        return _circles;
    }

    public void SetRadius(float newRadius)
    {
        _radius = newRadius;
    }

    [ContextMenu("Generate seed")]
    public void Generate(Vector3 position, float density, float desiredRadius)
    {
        _circles.Clear();

        var area = Mathf.Pow(_radius, 2) * Mathf.PI;
        var pointCount = area * _pointsPerUnit * density;

        for (var i = 0; i < pointCount; i++)
        {
            var point = Random.insideUnitCircle * _radius;
            var worldPoint = new Vector3(point.x, 0, point.y);

            _circles.Add(new Circle { Position = worldPoint + position, Radius = desiredRadius });
        }
    }

    void OnDrawGizmos()
    {
        for (var i = 0; i < _circles.Count; i++)
        {
            Gizmos.DrawSphere(_circles[i].Position, _circles[i].Radius);
        }
    }
}
