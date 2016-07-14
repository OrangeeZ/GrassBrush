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

        public WorldSpaceGridTileSet(WorldSpaceGrid<T> grid, int itemsPerTile)
        {
            Grid = grid;
            ItemsPerTile = itemsPerTile;

            var itemsX = Grid.ResolutionX / ItemsPerTile;
            var itemsZ = Grid.ResolutionZ / ItemsPerTile;
            
            TileCount= itemsX * itemsZ;

            _dirtyFlags = new bool[TileCount];
        }

        public void ForEachInPatch(int tileId, WorldSpaceGrid<T>.GridElementCallback callback)
        {
            var z = tileId / ItemsPerTile;
            var x = (tileId  - z * ItemsPerTile) % ItemsPerTile;

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
