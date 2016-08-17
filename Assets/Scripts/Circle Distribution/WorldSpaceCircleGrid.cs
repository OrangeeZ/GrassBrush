using System;
using System.Collections.Generic;
using BO.Utilities;
using UnityEngine;
using System.Collections;

public class WorldSpaceCircleGrid : WorldSpaceGrid<List<DistributedCircleGenerator.Circle>>
{
    public WorldSpaceCircleGrid(int resolutionX, int resolutionZ, Vector3 worldSize) : base(resolutionX, resolutionZ, worldSize)
    {
    }

    private DistributedCircleGenerator.Circle _circleToAdd;
    private Vector3 _intersectionPosition;
    private float _intersectionRadius;
    private bool _circleIntersectionResult;
    private Action<DistributedCircleGenerator.Circle> _circleCallback;

    public void AddCircle(DistributedCircleGenerator.Circle circle, float radius, int index)
    {
        _circleToAdd = circle;

        ForEachInRadius(circle.Position, radius, OnTryAddCircle);
    }

    public bool Intersects(DistributedCircleGenerator.Circle circle, float radius)
    {
        _circleIntersectionResult = false;

        _intersectionPosition = circle.Position;
        _intersectionRadius = radius;

        ForEachInRadius(circle.Position, radius, OnCircleIntersection);

        return _circleIntersectionResult;
    }

    private void OnCircleIntersection(List<DistributedCircleGenerator.Circle>[] items, int x, int z, int itemIndex)
    {
        var itemList = items[itemIndex];

        if (itemList == null)
        {
            return;
        }

        for (var j = 0; j < itemList.Count && !_circleIntersectionResult; j++)
        {
            _circleIntersectionResult = _circleIntersectionResult || IntersectCircles(itemList[j]);
        }
    }

    private bool IntersectCircles(DistributedCircleGenerator.Circle b)
    {
        return (_intersectionPosition - b.Position).sqrMagnitude <= Mathf.Pow(_intersectionRadius + b.Radius, 2);
    }

    private void OnTryAddCircle(List<DistributedCircleGenerator.Circle>[] items, int x, int z, int itemIndex)
    {
        items[itemIndex] = items[itemIndex] ?? new List<DistributedCircleGenerator.Circle>();
        items[itemIndex].Add(_circleToAdd);
    }

    public void RemoveCircle(Vector3 position, float radius, Action<DistributedCircleGenerator.Circle> circleCallback)
    {
        _intersectionPosition = position;
        _intersectionRadius = radius;
        _circleCallback = circleCallback;

        ForEachInRadius(position, radius, OnCircleRemove);
    }

    private void OnCircleRemove(List<DistributedCircleGenerator.Circle>[] items, int x, int z, int itemIndex)
    {
        var itemList = items[itemIndex];
        if (itemList == null)
        {
            return;
        }

        for (var i = 0; i < itemList.Count; i++)
        {
            if (IntersectCircles(itemList[i]))
            {
                _circleCallback(itemList[i]);

                itemList.RemoveAt(i);

                i = 0;
            }
        }

        //itemList.RemoveAll(_ => IntersectCircles(Circles[itemList[_]]));
    }
}
