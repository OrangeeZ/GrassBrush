using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class DetailObjectsData : ScriptableObject
{
    [SerializeField]
    private List<DistributedCircleGenerator.Circle> _detailObjects;

    public IList<DistributedCircleGenerator.Circle> GetDetailObjects()
    {
        return _detailObjects;
    }

    public void SetDetailObjects(IList<DistributedCircleGenerator.Circle> detailObjects)
    {
        _detailObjects = detailObjects.ToList();// detailObjects.Select<DistributedCircleGenerator.Circle, DistributedCircleGenerator.Circle>(InstanceSelector).ToList();
    }

    private DistributedCircleGenerator.Circle InstanceSelector(DistributedCircleGenerator.Circle _)
    {
        var result = new DistributedCircleGenerator.Circle
        {
            Position = _.Position,
            AngleY = _.AngleY,
            Scale = _.Scale,
            Radius = _.Radius,
            Instance = UnityEditor.PrefabUtility.GetPrefabParent(_.Instance) as DetailPreset
        };

        if (result.Instance == null)
        {
            Debug.LogError("null instance!");
        }

        return result;
    }
}
