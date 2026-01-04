using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UselessFrameUnity;

[CustomEditor(typeof(Entrance))]
public partial class EntranceEditor : Editor
{
    private string _settingPath = "Assets/Framework/Settings/Setting.asset";
    private FrameworkSetting _setting;
    private List<IDataEditor> _editors;

    private void OnEnable()
    {
        if (Application.isPlaying)
            return;

        _setting = AssetDatabase.LoadAssetAtPath<FrameworkSetting>(_settingPath);
        if (_setting == null)
        {
            _setting = CreateInstance<FrameworkSetting>();
            AssetDatabase.CreateAsset(_setting, _settingPath);
        }

        SerializedProperty dataProp = serializedObject.FindProperty("frameworkSetting");
        if (dataProp.objectReferenceValue != _setting)
        {
            dataProp.objectReferenceValue = _setting;
            serializedObject.ApplyModifiedProperties();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }

        _editors = new List<IDataEditor>()
        {
            new DebugEditor()
        };
        foreach (IDataEditor editor in _editors)
            editor.OnInit(_setting);
    }

    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
            return;

        if (_editors == null)
            return;

        foreach (IDataEditor editor in _editors)
        {
            editor.Enable = Utility.Toggle(editor.Name, editor.Enable);
            if (editor.Enable)
            {
                EditorGUILayout.BeginVertical(GUI.skin.customStyles[40]);
                editor.OnUpdate();
                EditorGUILayout.EndVertical();
            }
        }
    }


    private void OnDisable()
    {
        if (Application.isPlaying)
            return;

        if (_editors == null)
            return;

        foreach (IDataEditor editor in _editors)
        {
            editor.OnDestroy();
        }

        EditorUtility.SetDirty(_setting);
        _editors = null;
    }
}
