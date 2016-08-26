﻿using System;
using System.Collections.Generic;
using BO.Utilities;
using UnityEngine;
using System.Collections;

namespace BO.Client.Graphics.DetailObjects
{
    public class WorldSpaceCircleGrid : WorldSpaceGrid<List<DistributedCircleGenerator.Circle>>
    {
        public WorldSpaceCircleGrid(int resolutionX, int resolutionZ, Vector3 worldSize)
            : base(resolutionX, resolutionZ, worldSize)
        {
        }

        private DistributedCircleGenerator.Circle _circleToAdd;
        private Vector3 _intersectionPosition;
        private float _intersectionRadius;
        private bool _circleIntersectionResult;
        private Action<DistributedCircleGenerator.Circle> _circleCallback;

        public void AddCircle(DistributedCircleGenerator.Circle circle)
        {
            _circleToAdd = circle;

            ForEachInRadius(circle.Position, circle.Radius, OnTryAddCircle);
        }

        public bool Intersects(DistributedCircleGenerator.Circle circle, float spacing)
        {
            _circleIntersectionResult = false;

            _intersectionPosition = circle.Position;
            _intersectionRadius = circle.Radius + spacing;

            ForEachInRadius(circle.Position, circle.Radius + spacing, OnCircleIntersection);

            return _circleIntersectionResult;
        }

        private void OnCircleIntersection(List<DistributedCircleGenerator.Circle>[] items, int x, int z, int itemIndex)
        {
            if (_circleIntersectionResult)
            {
                return;
            }

            var itemList = items[itemIndex];

            if (itemList == null)
            {
                return;
            }

            for (var j = 0; j < itemList.Count; j++)
            {
                _circleIntersectionResult = _circleIntersectionResult || IntersectCircles(itemList[j]);
            }
        }

        private bool IntersectCircles(DistributedCircleGenerator.Circle b)
        {
            var delta = _intersectionPosition - b.Position;
            delta.y = 0;

            return delta.sqrMagnitude < Mathf.Pow(_intersectionRadius + b.Radius, 2);
        }

        private void OnTryAddCircle(List<DistributedCircleGenerator.Circle>[] items, int x, int z, int itemIndex)
        {
            items[itemIndex] = items[itemIndex] ?? new List<DistributedCircleGenerator.Circle>();
            items[itemIndex].Add(_circleToAdd);
        }

        public void RemoveCircle(Vector3 position, float radius,
                                 Action<DistributedCircleGenerator.Circle> circleCallback)
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

                    i = -1;
                }
            }
        }

        public void OnDrawGizmos()
        {
            var items = GetItemsRaw();
            for (var i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    continue;
                }

                for (var j = 0; j < items[i].Count; j++)
                {
                    Gizmos.DrawSphere(items[i][j].Position, items[i][j].Radius);
                }
            }
        }
    }
}