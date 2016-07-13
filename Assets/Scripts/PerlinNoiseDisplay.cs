using UnityEngine;
using System.Collections;

public class PerlinNoiseDisplay : MonoBehaviour
{
    public float PerlinCoordinateScale = 0.5f;

    public int Resolution = 1024;

    public Texture2D Preview;

    [ContextMenu("Generate")]
    public void Generate()
    {
        DestroyImmediate(Preview);

        Preview = new Texture2D(Resolution, Resolution, TextureFormat.RGB24, mipmap: false, linear: true);
        var pixels = Preview.GetPixels32();

        var simplexNoise = new BO.Rendering.Utilities.OpenSimplexNoise();

        for (var x = 0; x < Resolution; x++)
        {
            for (var y = 0; y < Resolution; y++)
            {
                var worldPosition = transform.position + new Vector3(x, 0, y);
                var perlinValue = (float)simplexNoise.Evaluate(worldPosition.x * PerlinCoordinateScale, worldPosition.z * PerlinCoordinateScale);

                pixels[y * Resolution + x] = Color.white * Mathf.Pow(perlinValue, 2);
            }
        }

        Preview.SetPixels32(pixels);
        Preview.Apply();

        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = Preview;
    }
}
