using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Grass.Editor
{
    [CustomEditor(typeof(GrassBrush))]
    public class GrassBrushEditor : UnityEditor.Editor
    {
        private GrassBrush _target;

        void OnEnable()
        {
            _target = target as GrassBrush;
        }

        void OnSceneGUI()
        {
            var currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown || currentEvent.type == EventType.MouseDrag || currentEvent.type == EventType.MouseUp)
            {
                if (currentEvent.button == 0)
                {
                    var ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
                    var raycatHitInfo = default (RaycastHit);
                    var didHit = Terrain.activeTerrain.GetComponent<TerrainCollider>().Raycast(ray, out raycatHitInfo, float.MaxValue);

                    if (didHit)
                    {
                        _target.transform.position = raycatHitInfo.point;
                        
                        if (!currentEvent.shift)
                        {
                            _target.Draw();
                        }else
                        {
                            _target.Erase();
                        }

                        Selection.activeObject = _target;

                        currentEvent.Use();
                    }
                }
            }
        }
    }
}
