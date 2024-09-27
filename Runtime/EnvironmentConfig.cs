using System;
using UnityEngine;

[Serializable]
public abstract class EnvironmentConfig
{
    [SerializeField] private string m_Name;
    public string Name => m_Name;
}