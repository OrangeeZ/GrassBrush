using UnityEngine;
using System.Collections;

public class PerlinNoiseDisplay : MonoBehaviour
{
    public float StepSize = 0.5f;
    public float PerlinCoordinateScale = 0.5f;

    void OnDrawGizmos()
    {
        for (var i = 0; i < 80; i++)
        {
            for (var j = 0; j < 80; j++)
            {
                var worldPosition = transform.position + new Vector3(i, 0, j) * StepSize;
                var perlinValue = Mathf.PerlinNoise(worldPosition.x * PerlinCoordinateScale, worldPosition.z * PerlinCoordinateScale);

                Gizmos.color = Color.Lerp(Color.black, Color.white, perlinValue);
                Gizmos.DrawSphere(worldPosition, StepSize * 0.5f);
            }
        }
    }
}
