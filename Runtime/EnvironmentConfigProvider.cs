﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
#endif

[Serializable]
public abstract class EnvironmentConfig
{
    [SerializeField] private string m_Name;
    public string Name => m_Name;
}

public abstract class EnvironmentConfigProvider : ScriptableObject
{
    public abstract void SaveChanges();
}

public abstract class EnvironmentConfigProvider<T> : EnvironmentConfigProvider where T : EnvironmentConfig
{
    [SerializeField] private Feature[] m_Features;
    [SerializeField] private T[] m_EnvironmentConfigs;
    [HideInInspector][SerializeField] private int m_ActiveEnvironmentIndex;
    
    protected T ActiveEnvironment => m_EnvironmentConfigs[m_ActiveEnvironmentIndex];
    
#if UNITY_EDITOR
    
    public int ChangeActiveEnvironment(int activeEnvironmentIndex)
    {
        var prevActiveEnvironmentIndex = m_ActiveEnvironmentIndex;
        
        if (activeEnvironmentIndex < 0)
            return prevActiveEnvironmentIndex;
        
        if (activeEnvironmentIndex >= m_EnvironmentConfigs.Length)
            return prevActiveEnvironmentIndex;

        m_ActiveEnvironmentIndex = activeEnvironmentIndex;
        SaveChanges();
        return prevActiveEnvironmentIndex;
    }

    public override void SaveChanges()
    {
        var activeEnvironment = m_EnvironmentConfigs[m_ActiveEnvironmentIndex];
        Apply(activeEnvironment);
        
        // There seems to be a possibility of this happening
        if (this == null) 
            return;
        
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
    }

    protected virtual void Apply(T environmentConfig)
    {
        var featureFlags = ExtractFeatureFlags(environmentConfig);
        UpdateFeatures(m_Features, featureFlags);
        UpdateScriptDefineSymbols();
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
    
    private void UpdateScriptDefineSymbols()
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        var buildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
        var scriptingDefineSymbolsAsText = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
        var scriptingDefineSymbols = new HashSet<string>(scriptingDefineSymbolsAsText.Split(';', StringSplitOptions.RemoveEmptyEntries));
        foreach (var feature in m_Features)
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
    
#endif

}

[AttributeUsage(AttributeTargets.Field)]
public sealed class FeatureFlagAttribute : Attribute
{
    public string FeatureName { get; }

    public FeatureFlagAttribute(string featureName)
    {
        FeatureName = featureName;
    }
}

[Serializable]
public sealed class Feature
{
    [SerializeField] private string m_Name;
    [Tooltip("This may be driven by a [FeatureFlag]")]
    [HideInInspector][SerializeField] private bool m_IsEnabled;
    [SerializeField] private List<string> m_ScriptDefineSymbols;
    [HideInInspector][SerializeField] private List<string> m_PrevScriptDefineSymbols;
    
    public string Name => m_Name;
    
    public bool IsEnabled
    {
        get => m_IsEnabled;
        set => m_IsEnabled = value;
    }

    public void UpdateScriptDefineSymbols(HashSet<string> symbols)
    {
        foreach (var prevScriptDefineSymbol in m_PrevScriptDefineSymbols)
        {
            symbols.Remove(prevScriptDefineSymbol);
        }
        m_PrevScriptDefineSymbols.Clear();
        m_PrevScriptDefineSymbols.AddRange(symbols);
        
        if (m_IsEnabled)
        {
            foreach (var symbol in m_ScriptDefineSymbols)
            {
                symbols.Add(symbol);
            }
        }
        else
        {
            foreach (var symbol in m_ScriptDefineSymbols)
            {
                symbols.Remove(symbol);
            }
        }
    }
}