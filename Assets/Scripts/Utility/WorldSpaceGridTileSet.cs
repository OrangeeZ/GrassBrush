using UnityEngine;
using System.Collections;

namespace BO.Utilities
{
    public class WorldSpaceGridTileSet<T>
    {
        public WorldSpaceGrid<T> Grid { get; set; }
        public int ItemsPerTile { get; private set; }
        public int TileCount { get; private set; }

        private bool[] _dirtyFlags;

        private int _tilesX;
        private int _tilesZ;

        public WorldSpaceGridTileSet(WorldSpaceGrid<T> grid, int itemsPerTile)
        {
            Grid = grid;
            ItemsPerTile = itemsPerTile;

            _tilesX = Grid.ResolutionX / ItemsPerTile;
            _tilesZ = Grid.ResolutionZ / ItemsPerTile;

            TileCount = _tilesX * _tilesZ;

            _dirtyFlags = new bool[TileCount];
        }

        public void ForEachInPatch(int tileId, WorldSpaceGrid<T>.GridElementCallback callback)
        {
            var z = tileId / _tilesZ;
            var x = (tileId - z * _tilesZ) % _tilesX;

            Grid.ForEachInRect(x * ItemsPerTile, z * ItemsPerTile, ItemsPerTile, ItemsPerTile, callback);
        }

        public int GetTileIdAtGridPosition(int x, int z)
        {
            var itemsZ = Grid.ResolutionZ / ItemsPerTile;
            var result = z / ItemsPerTile * itemsZ + x / ItemsPerTile;

            return result;
        }

        public bool GetTileDirty(int tileId)
        {
            return _dirtyFlags[tileId];
        }

        public void SetTileDirty(int tileId, bool isDirty)
        {
            _dirtyFlags[tileId] = isDirty;
        }
    }
}
