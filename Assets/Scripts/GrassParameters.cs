using System.Collections.Generic;
using BO.Rendering.Utilities;
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

        private OpenSimplexNoise _noise;

        private List<Object> _usedObjects = new List<Object>();

        [ContextMenu("Generate seed")]
        public void GenerateSeed()
        {
            _seed = Mathf.FloorToInt(Random.value * int.MaxValue);
        }

        public float GetScale(Vector3 worldPosition)
        {
            return Mathf.Lerp(_minGrassSize, _maxGrassSize, GetNoiseValueAtPoint(worldPosition));
        }

        public Vector3 GetOffset(Vector3 worldPosition)
        {
            Random.seed = _seed;

            var offset = Mathf.Lerp(-_maxOffset, _maxOffset, GetNoiseValueAtPoint(worldPosition));

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

            instance.transform.position = worldPosition + GetOffset(worldPosition);
            instance.transform.localScale = Vector3.one * scale;
            instance.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontSaveInEditor;

            return instance;
        }

        private byte AddPrefab(GameObject prefab)
        {
            var result = _usedObjects.IndexOf(prefab);
            if (result == -1)
            {
                result = _usedObjects.Count;
                _usedObjects.Add(prefab);
            }

            return (byte)result;
        }

        private float GetNoiseValueAtPoint(Vector3 worldPosition)
        {
            _noise = _noise ?? new OpenSimplexNoise(_seed);

            var result = (float)_noise.Evaluate(worldPosition.x * _perlinCoordinateScale, worldPosition.z * _perlinCoordinateScale);
            return Mathf.Pow(result, 2);
        }
    }
}