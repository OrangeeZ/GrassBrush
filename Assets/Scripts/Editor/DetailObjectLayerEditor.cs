using Grass;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor;

namespace Grass
{
    [CustomEditor(typeof(DetailObjectLayer))]
    public class DetailObjectLayerEditor : Editor
    {
        private DetailObjectLayer _target;

        private DetailObjectPresetList _presetList;

        void OnEnable()
        {
            _target = target as DetailObjectLayer;

            _presetList = new DetailObjectPresetList(serializedObject, serializedObject.FindProperty("Brushes"), _target);
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

            if (isMouseMoving)
            {
                if (currentEvent.button == 0)
                {
                    if (didHit)
                    {
                        brush.Position = raycatHitInfo.point;

                        if (!currentEvent.shift)
                        {
                            brush.Draw();
                        }
                        else
                        {
                            brush.Erase();
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


            //_target.ActiveBrushIndex = _presetList.ActiveBrushIndex;

            //base.OnInspectorGUI();
        }
    }
}

public class DetailObjectPresetList
{
    public int ActiveBrushIndex;

    public DetailObjectBrush ActiveBrush { get; private set; }
    private readonly SerializedObject _targetObject;
    private readonly SerializedProperty _targetProperty;
    private readonly DetailObjectLayer _detailObjectLayer;

    //public List<DetailObjectBrush> Brushes;

    private ReorderableList _presetList;

    private AnimBool _showScope;
    private SerializedProperty _activeBrush;

    public DetailObjectPresetList(SerializedObject targetObject, SerializedProperty targetProperty, DetailObjectLayer detailObjectLayer)
    {
        _targetObject = targetObject;
        _targetProperty = targetProperty;
        _detailObjectLayer = detailObjectLayer;
        _presetList = new ReorderableList(targetObject, targetProperty, draggable: false, displayHeader: false, displayAddButton: true, displayRemoveButton: true);

        _presetList.drawElementCallback = (rect, index, active, focused) =>
        {
            rect.x += 20;
            var element = targetProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, includeChildren: true);

            if (GUILayout.Button("V"))
            {
                //ActiveBrush = ele
            }
        };

        _presetList.elementHeightCallback = (index) =>
        {
            var element = _targetProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element);
        };

        _activeBrush = _targetObject.FindProperty("ActiveBrush");

        //_showScope = new AnimBool(repaintCallback);
    }

    public void OnInspectorGUI()
    {
        _targetObject.Update();

        EditorGUILayout.PropertyField(_activeBrush, includeChildren: true);

        if (GUILayout.Button("Add active preset"))
        {
            _detailObjectLayer.AddActivePreset();

            return;
        }

        for (var i = 0; i < _targetProperty.arraySize; i++)
        {
            var element = _targetProperty.GetArrayElementAtIndex(i);

            using (var horizontalScope = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("A", GUILayout.Width(20)))
                {
                    _detailObjectLayer.SetPresetActive(i);
                }

                

                GUILayout.Space(10);

                //var isActive = EditorGUILayout.Toggle(ActiveBrushIndex == i, GUILayout.Width(20));

                //if (isActive)
                //{
                //    ActiveBrushIndex = i;
                //}

                EditorGUILayout.PropertyField(element, includeChildren: true);

                GUILayout.Space(10);


                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _detailObjectLayer.RemovePreset(i);
                    return;
                }
            }
        }

        _targetObject.ApplyModifiedProperties();
        
        //_targetObject.Update();
        //_presetList.DoLayoutList();
        //_targetObject.ApplyModifiedProperties();
        //EditorGUILayout.PropertyField(_presetList.)
        //_showScope.target = EditorGUILayout.Foldout(_showScope.target, "Show extra fields");
        //using (var scope = new EditorGUILayout.FadeGroupScope(_showScope.faded))
        {
            //EditorGUILayout.PropertyField()
            //_showScope.target = scope.visible;
        }
    }
}
