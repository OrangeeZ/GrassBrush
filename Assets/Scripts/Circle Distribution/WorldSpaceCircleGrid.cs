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
    private DistributedCircleGenerator.Circle _circleToIntersect;
    private bool _circleIntersectionResult;

    public void AddCircle(DistributedCircleGenerator.Circle circle, int index)
    {
        _circleIndexToAdd = index;

        ForEachInRadius(circle.Position, circle.Radius, OnTryAddCircle);
    }

    public bool Intersects(DistributedCircleGenerator.Circle circle)
    {
        _circleIntersectionResult = false;

        _circleToIntersect = circle;

        ForEachInRadius(circle.Position, circle.Radius, OnCircleIntersection);

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
            _circleIntersectionResult = _circleIntersectionResult || IntersectCircles(_circleToIntersect, Circles[itemList[j]]);
        }
    }

    private bool IntersectCircles(DistributedCircleGenerator.Circle a, DistributedCircleGenerator.Circle b)
    {
        return (a.Position - b.Position).sqrMagnitude <= Mathf.Pow(a.Radius + b.Radius, 2);
    }

    private void OnTryAddCircle(List<int>[] items, int x, int z, int itemIndex)
    {
        items[itemIndex] = items[itemIndex] ?? new List<int>();
        items[itemIndex].Add(_circleIndexToAdd);
    }
}
