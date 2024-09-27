# What is envconfigs package?
The basic idea behind this package is to allow users to have multiple environment configurations and switch between them quickly and easialy.

For example, one might want to have a `local` vs `remote` environment configuration that specifies the URL or an API to be something different for each environment or a way to toggle features on and off, like cheats, based on if the build is being made for a `dev` or `prod` environment.

## Important pieces of this package

### Features
Features allow the user to specify a collection of Scripting Define Symbols that should be enabled or disabled together. This allows for quickly enabling and disabling certain features of a project. 

For example, one might have all code related to cheats surrounded with `#if ENABLE_CHEATS` define symbol. 

The user can create a `Feature` called `Cheats` which contains the `ENABLE_CHEATS` define symbol so that it may be toggled on and off based on the enviornment config.

### Feature Flags
A feature flag is a C# attribute that can be added to a boolean field to be used as a way to toggle a specific feature On or Off.

For example, one might add a bool field to their environment config to togle the above cheat feature like so:
```CS
[FeatureFlag("Cheats")]
[SerializeField] private bool m_IsCheatsFeatureEnabled;
```

### Enviroment Configs
The user can quickly switch between their environment configs using the inspector after selecting the environment config provider. 

![Image depicting how a user can switch environments via the insepctor](Documentation%20Images~%2Fswitchenvinspector.png)

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

5. Apply your changes