using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
#endif

public abstract class EnvironmentConfigProvider : ScriptableObject
{
    public abstract void ApplyChanges();
}

public abstract class EnvironmentConfigProvider<T> : EnvironmentConfigProvider
    where T : EnvironmentConfig
{
    [SerializeField] private Feature[] m_Features;
    [SerializeField] private T[] m_EnvironmentConfigs;
    [HideInInspector][SerializeField] private int m_ActiveEnvironmentIndex;
    
    private T ActiveEnvironment => m_EnvironmentConfigs[m_ActiveEnvironmentIndex];
    
#if UNITY_EDITOR
    
#pragma warning disable CS0414 // Used by EnvironmentConfigProviderEditor.cs script
    [HideInInspector][SerializeField] private bool m_IsDirty;
#pragma warning restore CS0414
    
    public int ChangeActiveEnvironment(int activeEnvironmentIndex)
    {
        var prevActiveEnvironmentIndex = m_ActiveEnvironmentIndex;
        
        if (activeEnvironmentIndex < 0)
            return prevActiveEnvironmentIndex;
        
        if (activeEnvironmentIndex >= m_EnvironmentConfigs.Length)
            return prevActiveEnvironmentIndex;

        m_ActiveEnvironmentIndex = activeEnvironmentIndex;
        ApplyChanges();
        return prevActiveEnvironmentIndex;
    }

    public override void ApplyChanges()
    {
        var activeEnvironment = ActiveEnvironment;
        Apply(m_Features, activeEnvironment);
        m_IsDirty = false;
        
        // There seems to be a possibility of this happening
        if (this == null) 
            return;
        
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
    }

    protected virtual void Apply(Feature[] features, T environmentConfig)
    {
        var featureFlags = ExtractFeatureFlags(environmentConfig);
        UpdateFeatures(features, featureFlags);
        UpdateScriptDefineSymbols(features);
        Debug.Log($"<b>{environmentConfig.Name}</b> config applied");
    }

    protected void UpdateFeatures(IEnumerable<Feature> features, Dictionary<string, bool> featureFlags)
    {
        foreach (var feature in features)
        {
            if (featureFlags.TryGetValue(feature.Name, out var isEnabled))
            {
                feature.IsEnabled = isEnabled;
            }
            else
            {
                feature.IsEnabled = false;
                Debug.LogWarning($"Disabling <b>{feature.Name}</b> feature because no feature flag found.\nPlease add <b>[FeatureFlag(\"{feature.Name}\")]</b> to your EnvironmentConfig");
            }
        }
    }

    protected Dictionary<string, bool> ExtractFeatureFlags(T environmentConfig)
    {
        var configType = environmentConfig.GetType();
        var featureFlags = configType
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute<FeatureFlagAttribute>() != null)
            .ToDictionary(f =>
            {
                var attrib = f.GetCustomAttribute<FeatureFlagAttribute>();
                return attrib.FeatureName;
            }, f => (bool)f.GetValue(environmentConfig));
        
        return featureFlags;
    }
    
    private void UpdateScriptDefineSymbols(IEnumerable<Feature> features)
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        var buildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
        var scriptingDefineSymbolsAsText = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
        var scriptingDefineSymbols = new HashSet<string>(scriptingDefineSymbolsAsText.Split(';', StringSplitOptions.RemoveEmptyEntries));
        foreach (var feature in features)
            feature.UpdateScriptDefineSymbols(scriptingDefineSymbols);

        var sb = new StringBuilder();
        foreach (var symbol in scriptingDefineSymbols)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                continue;
            
            sb.Append(symbol).Append(';');
        }

        var updatedScriptingDefineSymbolsAsText = string.Empty;
        if (sb.Length > 0)
            updatedScriptingDefineSymbolsAsText = sb.Remove(sb.Length - 1, 1).ToString(); //Remove the last ';'

        if (updatedScriptingDefineSymbolsAsText != scriptingDefineSymbolsAsText)
        {
            Debug.Log($"Updating Scripting Define Symbols...\nNew <b>{updatedScriptingDefineSymbolsAsText}</b>\nOld <b>{scriptingDefineSymbolsAsText}</b>");
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, updatedScriptingDefineSymbolsAsText);
        }
    }

    private void OnValidate()
    {
        //m_IsDirty = true;
    }
    
#endif

}