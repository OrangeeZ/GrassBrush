using System.Collections.Generic;
using Grass;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Tools/Editor/Detail Objects Layer/Create Presets Info")]
public class DetailObjectLayerPresetsInfo : ScriptableObject
{
    public List<DetailObjectBrush> Presets;
}
