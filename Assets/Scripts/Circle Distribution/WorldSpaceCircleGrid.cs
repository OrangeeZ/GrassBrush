using System;
using System.Collections.Generic;
using BO.Utilities;
using UnityEngine;
using System.Collections;

public class WorldSpaceCircleGrid : WorldSpaceGrid<List<int>>
{
    public WorldSpaceCircleGrid(int resolutionX, int resolutionZ, Vector3 worldSize) : base(resolutionX, resolutionZ, worldSize)
    {
    }

    public List<DistributedCircleGenerator.Circle> Circles; 

    private int _circleIndexToAdd;
    private Vector3 _intersectionPosition;
    private float _intersectionRadius;
    private bool _circleIntersectionResult;

    public void AddCircle(DistributedCircleGenerator.Circle circle, float radius, int index)
    {
        _circleIndexToAdd = index;

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

    private void OnCircleIntersection(List<int>[] items, int x, int z, int itemIndex)
    {
        var itemList = items[itemIndex];

        if (itemList == null)
        {
            return;
        }

        for (var j = 0; j < itemList.Count && !_circleIntersectionResult; j++)
        {
            _circleIntersectionResult = _circleIntersectionResult || IntersectCircles(Circles[itemList[j]]);
        }
    }

    private bool IntersectCircles(DistributedCircleGenerator.Circle b)
    {
        return (_intersectionPosition - b.Position).sqrMagnitude <= Mathf.Pow(_intersectionRadius + b.Radius, 2);
    }

    private void OnTryAddCircle(List<int>[] items, int x, int z, int itemIndex)
    {
        items[itemIndex] = items[itemIndex] ?? new List<int>();
        items[itemIndex].Add(_circleIndexToAdd);
    }
}
