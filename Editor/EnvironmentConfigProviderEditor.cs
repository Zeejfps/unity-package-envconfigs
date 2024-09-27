using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnvironmentConfigProvider), true)]
public class EnvironmentConfigProviderEditor : Editor
{
    private EnvironmentConfigProvider m_ConfigProvider;
    private SerializedProperty m_ActiveEnvironmentIndexProperty;
    private SerializedProperty m_EnvironmentsProperty;
    
    private void OnEnable()
    {
        m_ConfigProvider = (EnvironmentConfigProvider)target;
        m_ActiveEnvironmentIndexProperty = serializedObject.FindProperty("m_ActiveEnvironmentIndex");
        m_EnvironmentsProperty = serializedObject.FindProperty("m_EnvironmentConfigs");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var prevActiveEnvironmentIndex = m_ActiveEnvironmentIndexProperty.intValue;
        var displayOptions = GetEnvironmentConfigNames().ToArray();
        var currActiveEnvironmentIndex = EditorGUILayout.Popup("Active Environment", prevActiveEnvironmentIndex, displayOptions);
        m_ActiveEnvironmentIndexProperty.intValue = currActiveEnvironmentIndex;
        serializedObject.ApplyModifiedProperties();

        if (currActiveEnvironmentIndex != prevActiveEnvironmentIndex)
        {
            EditorUtility.SetDirty(m_ConfigProvider);
        }

        var isDirty = EditorUtility.IsDirty(m_ConfigProvider);
        EditorGUI.BeginDisabledGroup(!isDirty);
        if (GUILayout.Button("Save"))
        {
            m_ConfigProvider.SaveChanges();
        }
        EditorGUI.EndDisabledGroup();
    }

    private IEnumerable<string> GetEnvironmentConfigNames()
    {
        var environmentsCount = m_EnvironmentsProperty.arraySize;
        for (var i = 0; i < environmentsCount; i++)
        {
            var environmentProperty = m_EnvironmentsProperty.GetArrayElementAtIndex(i);
            yield return environmentProperty.FindPropertyRelative("m_Name").stringValue;
        }
    }
}