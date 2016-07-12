using UnityEngine;
using System.Collections;

namespace Grass
{
    public class GrassParameters : MonoBehaviour
    {
        [Header("Grass")]
        [SerializeField]
        private GameObject _grassPrefab;

        [Header("Size")]

        [SerializeField]
        private float _minGrassSize = 0.7f;

        [SerializeField]
        private float _maxGrassSize = 1.3f;

        [Header("Distribution")]

        [SerializeField]
        private int _seed;

        [SerializeField]
        private float _maxOffset = 0.1f;

        [SerializeField]
        private float _perlinCoordinateScale = 8f;

        [ContextMenu("Generate seed")]
        public void GenerateSeed()
        {
            _seed = Mathf.FloorToInt(Random.value * int.MaxValue);
        }

        public float GetScale(Vector3 worldPosition)
        {
            Random.seed = _seed;

            var perlinOffset = Random.insideUnitCircle;
            perlinOffset.x += worldPosition.x;
            perlinOffset.y += worldPosition.z;

            var perlinValue = Mathf.PerlinNoise(perlinOffset.x * 2, perlinOffset.y * 2);

            return Mathf.Lerp(_minGrassSize, _maxGrassSize, perlinValue);
        }

        public Vector3 GetOffset(Vector3 worldPosition)
        {
            Random.seed = _seed;

            var perlinOffset = Random.insideUnitCircle;
            perlinOffset.x += worldPosition.x;
            perlinOffset.y += worldPosition.z;

            var perlinValue = Mathf.PerlinNoise(perlinOffset.x * 16, perlinOffset.y * 16);
            var offset = Mathf.Lerp(-_maxOffset, _maxOffset, perlinValue);

            var unitCircleOffsetDirection = Random.insideUnitCircle.normalized;
            var result = new Vector3(unitCircleOffsetDirection.x, 0, unitCircleOffsetDirection.y);

            return result * offset;
        }

        public GameObject PlaceGrassAtPosition(Vector3 worldPosition, float density)
        {
            var perlinAtPoint = GetNoiseValueAtPoint(worldPosition);

            if (perlinAtPoint > density)
            {
                return null;
            }

            var instance = Instantiate(_grassPrefab);
            var scale = GetScale(worldPosition);

            worldPosition.y = Terrain.activeTerrain.SampleHeight(worldPosition) + instance.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * scale;
            instance.transform.position = worldPosition;

            instance.transform.localScale = Vector3.one * scale;

            return instance;
        }

        private float GetNoiseValueAtPoint(Vector3 worldPosition)
        {
            var result = Mathf.PerlinNoise(worldPosition.x * _perlinCoordinateScale, worldPosition.z * _perlinCoordinateScale);
            return result;
        }
    }
}