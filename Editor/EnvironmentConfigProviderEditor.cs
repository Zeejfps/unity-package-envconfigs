using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnvironmentConfigProvider), true)]
public sealed class EnvironmentConfigProviderEditor : Editor
{
    private EnvironmentConfigProvider m_ConfigProvider;
    private SerializedProperty m_ActiveEnvironmentIndexProperty;
    private SerializedProperty m_EnvironmentsProperty;
    private SerializedProperty m_IsDirtyProperty;
    
    private void OnEnable()
    {
        m_ConfigProvider = (EnvironmentConfigProvider)target;
        m_ActiveEnvironmentIndexProperty = serializedObject.FindProperty("m_ActiveEnvironmentIndex");
        m_EnvironmentsProperty = serializedObject.FindProperty("m_EnvironmentConfigs");
        m_IsDirtyProperty = serializedObject.FindProperty("m_IsDirty");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        var prevActiveEnvironmentIndex = m_ActiveEnvironmentIndexProperty.intValue;
        var displayOptions = GetEnvironmentConfigNames().ToArray();
        var currActiveEnvironmentIndex = EditorGUILayout.Popup("Active Environment", prevActiveEnvironmentIndex, displayOptions);
        m_ActiveEnvironmentIndexProperty.intValue = currActiveEnvironmentIndex;
        var changed = EditorGUI.EndChangeCheck();
        
        m_IsDirtyProperty.boolValue |= changed;
        serializedObject.ApplyModifiedProperties();
        
        var isDirty =  m_IsDirtyProperty.boolValue;
        EditorGUI.BeginDisabledGroup(!isDirty);
        if (GUILayout.Button("Apply"))
        {
            m_ConfigProvider.ApplyChanges();
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