# Projeto NULO – Developer Guide

## Overview
Projeto NULO is a Unity 3D stealth/horror game built with Unity 6000.0.60f1.

## Project Layout
| Path | Contents |
|------|----------|
| `Assets/Project/Scripts/` | All game C# source code |
| `Assets/Project/Prefabs/` | Reusable game object prefabs |
| `Assets/Project/Scenes/` | Unity scene files |
| `Assets/Project/Audio/` | Audio clips and mixer |
| `Assets/Project/UI/` | UI prefabs and sprites |
| `docs/` | Project documentation |

## Code Patterns

### Singleton (shared resources)
Managers that must exist only once in a scene use the Singleton pattern.
```csharp
public class MyManager : MonoBehaviour
{
    public static MyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```
Existing singletons: `GameManager`, `AudioManager`, `GameStateManager`, `UIPanelManager`, `ObjectFactory`.

### Factory (object creation)
`ObjectFactory` provides a centralised way to instantiate prefabs by string ID, decoupling
spawn logic from caller code.
```csharp
// Spawn an enemy at a given position
GameObject enemy = ObjectFactory.Instance.Create("Enemy", spawnPoint.position, Quaternion.identity);
```

### MVC (UI / menus)
UI panels follow a lightweight Model-View-Controller split:
- **Model** – data classes / ScriptableObjects holding state.
- **View** – Unity UI components (`Text`, `Image`, `Slider`, …).
- **Controller** – MonoBehaviours that read model state and update the view.

## Getting Started
1. Open the project in **Unity 6000.0.60f1**.
2. Load `Assets/Project/Scenes/Game Scene.unity`.
3. Press **Play** to run the game in the Editor.
