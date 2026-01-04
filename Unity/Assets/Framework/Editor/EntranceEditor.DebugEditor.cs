
using Mono.Cecil;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UselessFrameUnity;

public partial class EntranceEditor
{
    private class DebugEditor : DataEditorBase
    {
        private const string DEBUG = "CONSOLE";
        private string[] m_LogHelperTypeNames;
        private int m_LogHelperTypeIndex;
        private Vector2 m_LogScrollPos;
        private bool m_MoreColorDetail;

        protected override void OnInit()
        {
            m_MoreColorDetail = false;
        }

        public override void OnUpdate()
        {
            #region All Color Select
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            Utility.Lable("Log Colors");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.customStyles[40]);
            List<DebugColor> colors = m_Data.LogMark;
            #region Calculate Select
            EditorGUILayout.BeginHorizontal();
            bool all = true;
            bool dirty = false;
            if (colors == null)
            {
                colors = new List<DebugColor>();
                m_Data.LogMark = colors;
            }
            foreach (DebugColor data in colors)
            {
                if (data.Value)
                    data.Color = EditorGUILayout.ColorField(new GUIContent(""), data.Color, false, false, false, GUILayout.Width(20));
                if (!data.Value)
                    all = false;
            }
            bool olds = all;
            all = EditorGUILayout.Toggle(all, GUILayout.Width(15));
            if (olds != all)
                dirty = true;

            if (GUILayout.Button("x", GUILayout.Width(20)))
                colors.Clear();

            GUILayout.FlexibleSpace();
            m_MoreColorDetail = Utility.Toggle(m_MoreColorDetail);
            EditorGUILayout.EndHorizontal();
            #endregion


            if (m_MoreColorDetail)
            {
                m_LogScrollPos = EditorGUILayout.BeginScrollView(m_LogScrollPos);
                List<int> willDel = new List<int>();
                for (int i = 0; i < colors.Count; i++)
                {
                    DebugColor logMark = colors[i];
                    if (dirty) logMark.Value = all;
                    EditorGUILayout.BeginHorizontal();
                    logMark.Key = EditorGUILayout.TextField(logMark.Key);
                    logMark.Color = EditorGUILayout.ColorField(logMark.Color, GUILayout.Width(40));
                    logMark.Value = EditorGUILayout.Toggle(logMark.Value);
                    if (GUILayout.Button("x"))
                        willDel.Add(i);
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("+"))
                {
                    DebugColor data = new DebugColor();
                    data.Value = true;
                    colors.Add(data);
                }

                foreach (int i in willDel)
                    colors.RemoveAt(i);
                EditorGUILayout.EndScrollView();
            }
            else
            {
                foreach (DebugColor data in colors)
                    if (dirty) data.Value = all;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            #endregion
        }

        private void InnerSelect(int index)
        {
            m_LogHelperTypeIndex = index;
            EditorUtility.SetDirty(m_Data);
        }
    }
}