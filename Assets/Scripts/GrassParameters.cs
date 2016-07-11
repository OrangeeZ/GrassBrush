using UnityEngine;
using System.Collections;

namespace Grass
{
	public class GrassParameters : MonoBehaviour
	{

		[Header ("Size")]

		[SerializeField]
		private float _minGrassSize = 0.7f;

		[SerializeField]
		private float _maxGrassSize = 1.3f;

		[Header ("Distribution")]

		[SerializeField]
		private int _seed;

		[SerializeField]
		private float _maxOffset = 0.1f;

		[ContextMenu("Generate seed")]
		public void GenerateSeed ()
		{
			_seed = Mathf.FloorToInt(Random.value * int.MaxValue);
		}

		public float GetScale (Vector3 worldPosition)
		{
			Random.seed = _seed;

			var perlinOffset = Random.insideUnitCircle;
			perlinOffset.x += worldPosition.x;
			perlinOffset.y += worldPosition.z;

			var perlinValue = Mathf.PerlinNoise (perlinOffset.x, perlinOffset.y);

			return Mathf.Lerp(_minGrassSize, _maxGrassSize, perlinValue);
		}

		public Vector3 GetOffset (Vector3 worldPosition)
		{
			Random.seed = _seed;

			var perlinOffset = Random.insideUnitCircle;
			perlinOffset.x += worldPosition.x;
			perlinOffset.y += worldPosition.z;

			var perlinValue = Mathf.PerlinNoise (perlinOffset.x, perlinOffset.y);
			var offset = Mathf.Lerp (-_maxOffset, _maxOffset, perlinValue);

			var unitCircleOffsetDirection = Random.insideUnitCircle.normalized;
			var result = new Vector3 (unitCircleOffsetDirection.x, 0, unitCircleOffsetDirection.y);

			return result * offset;
		}
	}
}