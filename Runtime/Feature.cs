using System;
using System.Collections.Generic;
using UnityEngine;

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
        foreach (var symbol in m_PrevScriptDefineSymbols)
        {
            symbols.Remove(symbol);
        }
        m_PrevScriptDefineSymbols.Clear();
        m_PrevScriptDefineSymbols.AddRange(m_ScriptDefineSymbols);
        
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