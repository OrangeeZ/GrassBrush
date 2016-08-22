using UnityEngine;
using UnityEditor;
using System.Collections;
using Grass;

namespace Grass
{
    [CustomEditor(typeof(DetailObjectLayer))]
    public class DetailObjectLayerEditor : Editor
    {
        private DetailObjectLayer _target;

        private DetailObjectBrush _brush;

        void OnEnable()
        {
            _target = target as DetailObjectLayer;
            _brush = _target.Brush;
        }

        void OnSceneGUI()
        {
            Selection.activeObject = _target;

            var currentEvent = Event.current;
            
            var ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            var raycatHitInfo = default(RaycastHit);
            var didHit = Terrain.activeTerrain.GetComponent<TerrainCollider>().Raycast(ray, out raycatHitInfo, float.MaxValue);

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            var isMouseMoving = currentEvent.type == EventType.MouseDown || currentEvent.type == EventType.MouseDrag || currentEvent.type == EventType.MouseUp;

            if (isMouseMoving)
            {
                if (currentEvent.button == 0)
                {
                    if (didHit)
                    {
                        _brush.Position = raycatHitInfo.point;

                        if (!currentEvent.shift)
                        {
                            _brush.Draw();
                        }
                        else
                        {
                            _brush.Erase();
                        }
                    }

                    currentEvent.Use();
                }
            }

            Handles.CircleCap(-1, raycatHitInfo.point, Quaternion.AngleAxis(90f, Vector3.right), _brush.Radius);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
