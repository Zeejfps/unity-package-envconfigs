using System;

[AttributeUsage(AttributeTargets.Field)]
public sealed class FeatureFlagAttribute : Attribute
{
    public string FeatureName { get; }

    public FeatureFlagAttribute(string featureName)
    {
        FeatureName = featureName;
    }
}