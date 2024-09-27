# What is envconfigs package?
TODO: Description of the package...

### Features
Features allow the user to specify a collection of Scripting Define Symbols that should be enabled or disabled together. This allows for quickly enabling and disabling certain features of a project. 

For example, one might have all the code related to cheats used by the developer surrounded with an #if ENABLE_CHEATS define symbol. 

The user can create a Feature called Cheats which list the ENABLE_CHEATS define symbol so that it may be toggled on and off based on the enviornment config.

### Feature Flags
A feature flag is a C# attribute that can be added to a boolean field to be used as a way to toggle a specific feature On or Off.

For example, one might add a bool field to their environment config to togle the above cheat feature like so:
```CS
[FeatureFlag("Cheats")]
[SerializeField] private bool m_IsCheatsFeatureEnabled;
```

### Active Enviroment
The user can quickly switch between their environment configs using the inspector after selecting the environment config provider. 

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

    ![Image depicting how to create the EnviromentConfigProvider](Documentation%20Images~%2Fcreateenvconfigprovider.png)

4. Setup your Features and Environments 

    ![Image depicting how the EnvironmentConfigProvider looks like in the inspector](Documentation%20Images~%2Fexampleenvconfigproviderinspector.png)
