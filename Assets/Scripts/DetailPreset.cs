using UnityEngine;
using System.Collections;

public class DetailPreset : MonoBehaviour {

    public float Radius;

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
