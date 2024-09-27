# What is envconfigs package?


# Sample Usage

1. Create a class that extends `EnvironmentConfig` 
    ```CS 
    [Serializable]
    public sealed class ExampleEnvironmentConfig : EnvironmentConfig
    {
        [SerializeField] private float m_ExampleFloatField;
        [SerializeField] private string m_ExapleStringField;
        [SerializeField] private bool m_ExampleBoolField;
        
        [FeatureFlag("Feature1")]
        [SerializeField] private bool m_ExampleFeature1Flag;
        
        [FeatureFlag("Feature2")]
        [SerializeField] private bool m_ExampleFeature2Flag;
    }
    ```
2. Create a class that extends `EnvironemntConfigProvider<T>`
    ```CS
    [CreateAssetMenu]
    public sealed class ExampleEnvironmentConfigProvider : EnvironmentConfigProvider<ExampleEnvironmentConfig>
    {
        
    }
    ```

3. Create your new EnvironmentConfigProvider in the project folder
