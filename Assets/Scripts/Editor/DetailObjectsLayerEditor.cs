using UnityEngine;
using UnityEditor;

namespace BO.Client.Graphics.DetailObjects
{
    [CustomEditor(typeof(DetailObjectsLayer))]
    public class DetailObjectsLayerEditor : Editor
    {
        private DetailObjectsLayer _target;

        private DetailObjectPresetList _presetList;

        void OnEnable()
        {
            _target = target as DetailObjectsLayer;

            if (_target.PresetsInfo == null)
            {
                var presetsInfo = EditorGUIUtility.Load("Detail Objects Layer/Default Presets Info.asset") as DetailObjectsLayerPresetsInfo;

                if (presetsInfo == null)
                {
                    presetsInfo = CreateInstance<DetailObjectsLayerPresetsInfo>();
                    AssetDatabase.CreateAsset(presetsInfo, "Assets/Editor Default Resources/Detail Objects Layer/Default Presets Info.asset");
                }

                _target.PresetsInfo = presetsInfo;

                if (_target.PresetsInfo.Presets.Count == 0)
                {
                    _target.PresetsInfo.Presets.Add(new DetailObjectsBrush());
                }
            }

            var targetProperty = new SerializedObject(_target.PresetsInfo);
            _presetList = new DetailObjectPresetList(targetProperty, targetProperty.FindProperty("Presets"), _target);
        }

        void OnSceneGUI()
        {
            Selection.activeObject = _target;

            var brush = _target.ActiveBrush;

            var currentEvent = Event.current;

            var ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            var raycatHitInfo = default(RaycastHit);
            var didHit = Terrain.activeTerrain.GetComponent<TerrainCollider>().Raycast(ray, out raycatHitInfo, float.MaxValue);

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            var isMouseMoving = currentEvent.type == EventType.MouseDrag || currentEvent.type == EventType.MouseUp;

            if (isMouseMoving && !currentEvent.alt)
            {
                if (currentEvent.button == 0)
                {
                    if (didHit)
                    {
                        brush.Position = raycatHitInfo.point;

                        if (!currentEvent.shift)
                        {
                            brush.Draw(_target);
                        }
                        else
                        {
                            brush.Erase(_target);
                        }
                    }

                    currentEvent.Use();
                }
            }

            Handles.CircleCap(-1, raycatHitInfo.point, Quaternion.AngleAxis(90f, Vector3.right), brush.Radius);
        }

        public override void OnInspectorGUI()
        {
            _presetList.OnInspectorGUI();

            EditorGUILayout.Space();

            var isManualEditModeActive = _target.ManualEditMode;

            if (GUILayout.Button(isManualEditModeActive ? "Disable manual edit mode" : "Enable manual edit mode"))
            {
                _target.ManualEditMode = !_target.ManualEditMode;
            }
        }
    }

    public class DetailObjectPresetList
    {
        public int ActiveBrushIndex;

        public DetailObjectsBrush ActiveBrush { get; private set; }
        private readonly SerializedObject _targetObject;
        private readonly SerializedProperty _targetProperty;
        private readonly DetailObjectsLayer _detailObjectsLayer;

        private GUIStyle _activeBrushStyle;
        private GUIStyle _removeButtonStyle;

        public DetailObjectPresetList(SerializedObject targetObject, SerializedProperty targetProperty, DetailObjectsLayer detailObjectsLayer)
        {
            _targetObject = targetObject;
            _targetProperty = targetProperty;
            _detailObjectsLayer = detailObjectsLayer;
        }

        public void OnInspectorGUI()
        {
            if (_activeBrushStyle == null)
            {
                _activeBrushStyle = new GUIStyle(GUI.skin.button)
                {
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.green }
                };

                _removeButtonStyle = new GUIStyle(_activeBrushStyle)
                {
                    normal = { textColor = Color.red }
                };
            }

            _targetObject.Update();

            for (var i = 0; i < _targetProperty.arraySize; i++)
            {
                var element = _targetProperty.GetArrayElementAtIndex(i);

                using (new EditorGUILayout.HorizontalScope())
                {
                    var isCurrentBrushActive = _detailObjectsLayer.ActiveBrushIndex == i;

                    var style = isCurrentBrushActive ? _activeBrushStyle : GUI.skin.button;

                    if (GUILayout.Button(new GUIContent("A", "Activate"), style, GUILayout.Width(20)))
                    {
                        _detailObjectsLayer.SetPresetActive(i);
                    }

                    if (GUILayout.Button(new GUIContent("D", "Duplicate"), GUILayout.Width(20)))
                    {
                        _detailObjectsLayer.DuplicatePreset(i);
                    }

                    GUILayout.Space(10);

                    EditorGUILayout.PropertyField(element, includeChildren: true);

                    GUILayout.Space(10);

                    if (GUILayout.Button(new GUIContent("X", "Remove"), _removeButtonStyle, GUILayout.Width(20)))
                    {
                        _detailObjectsLayer.RemovePreset(i);
                        break;
                    }
                }
            }

            _targetObject.ApplyModifiedProperties();
        }
    }
}
