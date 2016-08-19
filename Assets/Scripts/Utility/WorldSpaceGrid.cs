using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

namespace BO.Utilities
{
    public class WorldSpaceGrid<T>
    {
        public delegate void GridElementCallback(T[] items, int x, int z, int itemIndex);

        public int ResolutionX { get; private set; }
        public int ResolutionZ { get; private set; }

        private T[] _items;

        private Vector3 _worldSize;

        public WorldSpaceGrid(int resolutionX, int resolutionZ, Vector3 worldSize)
        {
            ResolutionX = resolutionX;
            ResolutionZ = resolutionZ;

            _items = new T[ResolutionX * ResolutionZ];
            _worldSize = worldSize;
        }

        public T[] GetItemsRaw()
        {
            return _items;
        }

        public void SetItemsRaw(T[] newItems)
        {
            Assert.AreEqual(newItems.Length, ResolutionX * ResolutionZ);
        }

        public void ForEachInRect(int x, int z, int itemsX, int itemsZ, GridElementCallback callback)
        {
            for (var currentZ = 0; currentZ <= itemsZ; currentZ++)
            {
                for (var currentX = 0; currentX <= itemsX; currentX++)
                {
                    var gridPositionX = currentX + x;
                    var gridPositionZ = currentZ + z;

                    var itemIndex = gridPositionZ * ResolutionX + gridPositionX;

                    if (itemIndex < 0 || itemIndex >= _items.Length)
                    {
                        continue;
                    }

                    callback(_items, gridPositionX, gridPositionZ, itemIndex);
                }
            }
        }

        public void ForEachInRadius(Vector3 worldPosition, float radius, GridElementCallback callback)
        {
            //var radiusX = WorldToGridX(radius);
            //var radiusZ = WorldToGridZ(radius);

            //var centerX = WorldToGridX(worldPosition.x);
            //var centerZ = WorldToGridZ(worldPosition.z);

            var fromX = WorldToGridX(worldPosition.x - radius);
            var toX = WorldToGridX(worldPosition.x + radius);

            var fromZ = WorldToGridZ(worldPosition.z - radius);
            var toZ = WorldToGridZ(worldPosition.z + radius);

            for (var x = fromX; x <= toX; x++)
            {
                for (var z = fromZ; z <= toZ; z++)
                {
                    //if (new Vector2(x, z).sqrMagnitude > radiusX * radiusZ)
                    //{
                    //    continue;
                    //}

                    //var gridPositionX = x + centerX;
                    //var gridPositionZ = z + centerZ;

                    var itemIndex = z * ResolutionX + x;

                    if (itemIndex < 0 || itemIndex >= _items.Length)
                    {
                        continue;
                    }

                    callback(_items, x, z, itemIndex);
                }
            }
        }

        private int WorldToGridX(float worldX)
        {
            return Mathf.FloorToInt(worldX / _worldSize.x * ResolutionX);
        }

        private int WorldToGridZ(float worldZ)
        {
            return Mathf.FloorToInt(worldZ / _worldSize.z * ResolutionZ);
        }
    }
}
