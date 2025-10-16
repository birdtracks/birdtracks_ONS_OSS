# ONS_OSS - Object Naming Scenes Assessment Engine

## Project Overview

**ONS_OSS** (Object Naming Scenes - Open Source Software) is a set of Unity-based cognitive assessment games designed to be embedded within Flutter applications. The project provides voice-based assessment tasks where users describe visual scenes and stories, enabling evaluation of language production and cognitive abilities.

### Game Description
The application presents users with animated visual scenarios and prompts them to describe what they see. Each assessment consists of carefully designed scenes with accompanying video animations that tell short stories, requiring verbal responses from participants.

### Target Platforms

- **Android**: API Level 22+ (Android 5.1 / Lollipop)

### Unity Version
- **Unity 2022.3.62f2 LTS**

### Key Features
- **Voice Assessment Tasks (VAD)**: Four distinct visual assessment scenarios with video animations
  - Scene 1: Nest
  - Scene 2: Green Dress Story
  - Scene 3: Cats
  - Scene 4: Paw Prints
- **Flutter Integration**: Seamless Unity-Flutter communication for session management and data submission
- **Tutorial System**: "Explainer" system with animated character guidance
- **Analytics & Data Submission**: Real-time submission of assessment results to backend via Flutter
- **Service-Based Architecture**: Modular, ScriptableObject-based service system
- **Custom Animation System**: Lightweight tween animation framework

---

## Technical Architecture

### Core Systems

#### 1. **ServiceAsset Pattern**
A custom service architecture (Assets/_/Scripts/Core/Service/ServiceAsset.cs:8) provides:
- ScriptableObject-based services with automatic initialization
- Persistent services across scenes (`DontDestroyOnLoad`)
- Async initialization with lifecycle management
- Examples: `PlaySessionService`, `LoadingScreenService`, `UserProfileService`

#### 2. **PlaySession Management**
The `PlaySessionService` (Assets/_/Scripts/Core/PlaySession/PlaySessionService.cs:12) orchestrates game flow:
- Queue-based assessment task system
- Scene loading and unloading
- Loading screen management during transitions
- Data submission to Flutter via `UnityMessageManager`

#### 3. **Event System**
ScriptableObject-based event system (Assets/_/Scripts/Core/Event/GameEvent.cs:9):
- Asset-based events for cross-scene communication
- Observer pattern with subscribe/unsubscribe functionality
- Runtime-safe (Editor-only execution prevention)

#### 4. **Flutter Integration**
Bidirectional Unity-Flutter communication:
- `FlutterIntegration` component for initialization
- `UnityMessageManager` for message passing
- `TrackosaurusAPI` for result submission
- JSON serialization via Newtonsoft.Json

#### 5. **Analytics & Data Submission**
- `VADAnalytics` tracks assessment events
- `TrackosaurusAPI` submits results with session metadata
- Real-time data transmission to Flutter host app
- Structured payload format for backend processing

#### 6. **Tween Animation System**
Custom lightweight animation components:
- `TweenPosition`, `TweenRotation`, `TweenScale`
- `TweenImageColor`, `TweenSpriteColor`
- Custom easing functions via `Easing` class

### Assembly Definitions

| Assembly | Purpose | Key Dependencies |
|----------|---------|------------------|
| `BirdTracks.Game.Core` | Core game systems, services, UI, analytics | FlutterUnityIntegration, Unity.Linq, TextMeshPro |
| `Birdtracks.Game.ONS` | Voice assessment task implementation | Core, Sweet.uTility, Cinemachine, Timeline |
| `FlutterUnityIntegration` | Unity-Flutter bridge | Newtonsoft.Json |
| `Sweet.uTility` | Coroutine management utilities | - |
| `Unity.Linq` | GameObject LINQ query library | - |

### Design Patterns

- **Singleton**: ServiceAsset instances (e.g., `PlaySessionService.Instance`)
- **Observer**: GameEvent system for decoupled communication
- **Service Locator**: ServiceAsset pattern for accessing services
- **Queue**: PlaySessionService uses queues for task management
- **Component**: MonoBehaviour-based task logic

---

## Third-Party Packages & Plugins

### Unity Packages (from Packages/manifest.json)

| Package | Version | Purpose |
|---------|---------|---------|
| **Cinemachine** | 2.10.5 | Virtual camera system for cutscenes and camera animations |
| **TextMesh Pro** | 3.0.7 | Advanced text rendering for UI |
| **Unity Analytics** | 6.1.1 | Usage analytics and telemetry |
| **Timeline** | 1.7.7 | Sequencing animations and cutscenes in assessment scenes |
| **2D PSD Importer** | 8.1.1 | Import layered Photoshop files as sprite assets |

### Third-Party Libraries

- **Newtonsoft.Json (Json.NET)**: JSON serialization for Flutter-Unity communication
- **LINQtoGameObject**: LINQ extension methods for GameObject hierarchy queries
- **Sweet.uTility**: Custom utility library for coroutine management and helper methods

### Package Usage

- **Cinemachine**: Animated camera movements in tutorial sequences
- **Timeline**: Orchestrating assessment scene animations and timing
- **TextMesh Pro**: All UI text rendering

---

## Prerequisites

### Required Software

1. **Unity Hub** (latest version recommended)
2. **Unity 2022.3.62f2 LTS** - [Download from Unity Archive](https://unity.com/releases/editor/archive)
3. **Git** for version control

### Unity Modules Required

When installing Unity 2022.3.62f2, ensure these modules are installed:


- ✅ **Android Build Support**
  - Android SDK & NDK Tools
  - OpenJDK
- ✅ **Code Editor** (Visual Studio, Visual Studio Code, or Rider)


#### For Android Development:
- **Android SDK** (included via Unity or Android Studio)
- **JDK 11** (OpenJDK, included with Unity)
- **Android Device or Emulator** running API Level 22+ (Android 5.1+)

### Third-Party Accounts

- **Unity ID** (for Unity Services and Analytics)
- Backend API access (if TrackosaurusAPI requires authentication)
- Flutter app project credentials (for embedding Unity module)

---

## Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd ONS_OSS
```

### 2. Open in Unity

1. Launch **Unity Hub**
2. Click **Add** → **Add project from disk**
3. Navigate to the `ONS_OSS` folder and select it
4. Verify Unity version shows **2022.3.62f2**
   - If not installed, click the version dropdown and select **Install Editor**
5. Click the project name to open

### 3. Initial Project Import

- Unity will import all assets (first import may take 5-15 minutes)
- Wait for script compilation to complete
- Check the Console window for any errors

### 4. Verify Package Installation

Packages should auto-install from `Packages/manifest.json`. To verify:

1. Open **Window → Package Manager**
2. Confirm these packages are installed:
   - Cinemachine 2.10.5
   - TextMesh Pro 3.0.7
   - Unity Analytics 6.1.1
   - Timeline 1.7.7
   - 2D PSD Importer 8.1.1

If missing, Unity will typically prompt for automatic installation.

### 5. Import TextMesh Pro Essentials

If prompted on first run:

1. Go to **Window → TextMeshPro → Import TMP Essential Resources**
2. Click **Import**

### 6. Configure Project Settings

#### Bundle Identifier (Required for Builds)

1. Go to **Edit → Project Settings → Player**
2. Set **Company Name**: `YourCompanyName`
3. Under **Other Settings**, configure:
   - **iOS Bundle Identifier**: `com.yourcompany.ons` (or your preference)
   - **Android Package Name**: `com.yourcompany.ons`

#### Unity Services (Optional)

To enable Unity Analytics:

1. Go to **Edit → Project Settings → Services**
2. Link to an existing Unity project or create a new one
3. Enable **Analytics** service

### 7. Flutter Integration Setup

This Unity project is designed as an embedded module within a Flutter application:

1. **Export Unity as Flutter Module**: Follow Flutter-Unity-Widget integration guides
2. **Configure Flutter Project**: Add Unity view container to Flutter app
3. **Initialize Unity**: Pass `FlutterInitArgs` JSON with session data from Flutter
4. **Handle Callbacks**: Listen for result submissions from Unity via message channel

### Common Setup Issues

#### Issue: "Assembly definition cannot be loaded"
**Solution**: Delete the `Library` folder and restart Unity to force reimport.

#### Issue: Missing script references or compilation errors
**Solution**:
1. Verify all packages are installed in Package Manager
2. Go to **Assets → Reimport All**
3. Restart Unity

#### Issue: "The type or namespace 'Newtonsoft' could not be found"
**Solution**: Verify `Assets/FlutterIntergration/JsonDotNet` contains Newtonsoft.Json DLL files.

#### Issue: "TextMeshPro is not imported"
**Solution**: Go to **Window → TextMeshPro → Import TMP Essential Resources**

---

## Build Instructions

### Android Build

#### 1. Switch Platform
1. Go to **File → Build Settings**
2. Select **Android**
3. Click **Switch Platform** (if not already selected)

#### 2. Configure Player Settings
1. In Build Settings, click **Player Settings**
2. Navigate to **Player → Other Settings**:
   - **Package Name**: `com.yourcompany.ons`
   - **Minimum API Level**: Android 5.1 'Lollipop' (API level 22)
   - **Target API Level**: Automatic (Highest Installed) or latest
   - **Scripting Backend**: IL2CPP (recommended for performance)
   - **Target Architectures**: ✅ ARMv7, ✅ ARM64
   - **Internet Access**: Require (for analytics)

#### 3. Build
1. Return to **File → Build Settings**
2. Click **Build** or **Build And Run**
3. Select output folder for APK
4. Wait for build completion

### Export for Flutter Integration

For embedding Unity in Flutter apps:

1. Use the **Unity as a Library** workflow
2. Export builds for iOS and Android following Flutter-Unity-Widget documentation
3. Configure Flutter project to reference Unity module
4. Initialize Unity with proper session data via `FlutterIntegration.Init()`

---

## Project Structure

### Root Directory

```
ONS_OSS/
├── Assets/                    # All game assets and scripts
├── Library/                   # Unity cache (ignored by version control)
├── Logs/                      # Unity logs
├── Packages/                  # Package manifest and configuration
├── ProjectSettings/           # Project-wide settings
├── Temp/                      # Temporary build files
├── UserSettings/              # User-specific settings (ignored)
├── *.csproj                   # C# project files (auto-generated)
├── ONS_OSS.sln               # Visual Studio solution (auto-generated)
└── README.md                  # This file
```

### Assets Folder Structure

```
Assets/
├── _/                                          # Main game content
│   ├── Content/                               # Art, audio, and asset files
│   │   ├── Core/                             # Core game assets
│   │   │   ├── Data/                        # ScriptableObject assets (services, config)
│   │   │   ├── DebugPanel/                  # Debug UI assets
│   │   │   ├── Fonts/                       # Font files
│   │   │   ├── Prefabs/                     # Reusable game objects
│   │   │   ├── Scenes/                      # Core scenes (Empty, LoadingScreen, etc.)
│   │   │   ├── Sprites/                     # UI sprites and icons
│   │   │   └── Textures/                    # Texture assets
│   │   │
│   │   ├── Explainers/                       # Tutorial system assets
│   │   │   ├── Animations/                  # Character animations
│   │   │   ├── Audio/                       # Tutorial voiceovers
│   │   │   ├── Materials/                   # Character materials
│   │   │   └── Textures/                    # Character textures
│   │   │
│   │   ├── LoadingScreen/                    # Loading screen assets
│   │   │   ├── Animation/                   # Loading animations
│   │   │   └── Materials/                   # Loading screen materials
│   │   │
│   │   ├── Matching/                         # Matching game assets (placeholder)
│   │   │
│   │   └── Voice/                            # Voice assessment assets
│   │       ├── Animations/                  # Scene animations
│   │       ├── Art/                         # Scene artwork
│   │       ├── Audio/                       # Audio clips (payoff sounds)
│   │       ├── Data/                        # VoiceAssessmentOptions configs
│   │       ├── Materials/                   # Scene materials
│   │       ├── Prefabs/                     # Voice task prefabs
│   │       ├── Scenes/                      # 4 VAD assessment scenes
│   │       ├── Scripts/                     # Voice task scripts (assembly)
│   │       ├── SpriteSheets/                # Sprite atlases
│   │       ├── Textures/                    # Scene textures
│   │       ├── Timeline/                    # Timeline assets for animations
│   │       └── Videos/                      # Video assets (for VAD assessments)
│   │
│   └── Scripts/                               # All C# scripts
│       ├── Core/                              # Core game systems (assembly)
│       │   ├── Analytics/                    # Flutter integration, analytics
│       │   ├── Effects/                      # Tween animations
│       │   ├── Event/                        # Event system
│       │   ├── Games/                        # Scene loading utilities
│       │   ├── Input/                        # Input configuration
│       │   ├── Log/                          # Logging system
│       │   ├── PlaySession/                  # Session management
│       │   ├── Reference/                    # Reference containers (Camera, Canvas, Transform)
│       │   ├── Service/                      # ServiceAsset architecture
│       │   ├── UI/                           # Core UI components
│       │   └── Variable/                     # ScriptableObject variables
│       │
│       ├── Explainers/                        # Tutorial system scripts
│       │
│       └── Tasks/                             # Assessment task implementations
│           └── Voice/                        # Voice assessment scripts (assembly)
│
├── FlutterIntergration/                       # Flutter-Unity bridge
│   ├── JsonDotNet/                           # Newtonsoft.Json library
│   ├── NativeAPI.cs
│   ├── SingletonMonoBehaviour.cs
│   ├── UnityMessageManager.cs
│   └── FlutterUnityIntegration.asmdef
│
├── LINQtoGameObject/                          # GameObject LINQ library
│
├── Scenes/                                    # Main entry scene
│   └── SampleScene.unity                     # Primary scene (entry point)
│
├── StreamingAssets/                           # Runtime-loaded assets
│   ├── Anim_Green_Dress_Story_03C.mp4       # Video for Scene 2
│   └── Anim_Muddy_Paws.mp4                  # Video for Scene 4
│
├── Sweet.uTility/                             # Coroutine utility library
│
└── TextMesh Pro/                              # TMP resources and fonts
    ├── Documentation/
    ├── Fonts/
    ├── Resources/
    ├── Shaders/
    └── Sprites/
```

### Key Asset Locations

| Asset Type | Location |
|------------|----------|
| **Entry Scene** | `Assets/Scenes/SampleScene.unity` |
| **Core Services** | `Assets/_/Content/Core/Data/` |
| **Assessment Scenes** | `Assets/_/Content/Voice/Scenes/` |
| **Core Scripts** | `Assets/_/Scripts/Core/` |
| **Voice Scripts** | `Assets/_/Scripts/Tasks/Voice/` |
| **Prefabs** | `Assets/_/Content/Core/Prefabs/` and `Assets/_/Content/Voice/Prefabs/` |
| **Video Assets** | `Assets/StreamingAssets/` |
| **Audio** | `Assets/_/Content/Voice/Audio/` and `Assets/_/Content/Explainers/Audio/` |

---

## Development Notes

### Coding Conventions

- **Namespace**: All core code uses `BirdTracks.Game.Core`, voice tasks use `Birdtracks.Game.ONS`
- **Naming Conventions**:
  - Private fields: `_camelCase` with underscore prefix
  - Public properties: `PascalCase`
  - Local variables: `camelCase`
  - Methods: `PascalCase`
- **ScriptableObjects**: Used extensively for data, configuration, and services
- **Assembly Definitions**: Keep code modular; new scripts must be in appropriate assembly folders

### Testing the Game

#### In Unity Editor:

1. Open `Assets/_/Content/Core/Scenes/DebugPanel.unity`
2. Press **Play** in the Unity Editor
3. The game initializes with test `FlutterInitArgs` (configured in scene)
4. Use debug UI to navigate between assessment scenes
5. Press **T** key (if configured) to reinitialize

#### Testing Flutter Integration:

1. Build Unity as a library/module for Flutter
2. Run within Flutter app context with actual session data
3. Monitor Unity-Flutter messaging via Flutter DevTools
4. Verify result submissions reach backend

### Important Gotchas

#### 1. **Flutter Initialization Required**
- Unity expects `FlutterInitArgs` JSON on startup via `FlutterIntegration.Init()`
- In Editor mode, test arguments are provided automatically
- In production, Flutter **must** call this with real session data

#### 2. **ServiceAsset Lifecycle**
- ServiceAssets auto-initialize when enabled in play mode
- They persist across scenes using `DontDestroyOnLoad`
- Do not instantiate multiple instances; use singleton pattern

#### 3. **Scene Management**
- Main entry point: `SampleScene.unity`
- Assessment scenes loaded additively by `PlaySessionService`
- Empty scene used for cleanup between tasks

#### 4. **Assembly References**
- Scripts in different assemblies require `.asmdef` references
- Cannot create circular assembly dependencies
- New scripts must be in correct assembly folder or won't compile

#### 5. **Editor vs. Build Behavior**
- Many systems check `#if UNITY_EDITOR` for Editor-specific behavior
- Flutter messaging disabled in Editor mode
- Some features only work in device builds

#### 6. **Video Playback**
- Videos stored in `StreamingAssets` for runtime access
- Use `Application.streamingAssetsPath` to load
- Test on actual devices as performance varies

#### 7. **Analytics & Data Submission**
- All results submitted via `TrackosaurusAPI.SubmitStoryGameResult()`
- Data passed to Flutter via `UnityMessageManager`
- Only functions in builds (disabled in Editor)

### Debugging Tips

- **Unity Console**: Monitor errors, warnings, and logs during play
- **Flutter DevTools**: Inspect Unity-Flutter message passing
- **Unity Profiler**: Analyze performance (**Window → Analysis → Profiler**)
- **LogAsset**: Configure per-system logging levels (see `Log.Default.asset`)
- **Debug Panel Scene**: Use for testing individual systems

### Performance Considerations

- **2D Rendering**: Lightweight sprite-based rendering
- **Custom Tweens**: No external animation library overhead
- **ScriptableObjects**: Reduce runtime memory allocations
- **Streaming Assets**: Videos loaded on-demand to manage memory
- **IL2CPP**: Recommended for production builds (better performance)

---

## Contributing

When contributing to this project:

1. Follow the established naming conventions and code style
2. Place new scripts in appropriate assembly definition folders
3. Add XML documentation comments (`///`) for public APIs
4. Test in both Unity Editor and on target platforms (iOS/Android)
5. Ensure Flutter integration still functions after changes
6. Update this README if adding significant features or dependencies

---

## License

[Specify your license here - e.g., MIT, Apache 2.0, GPL, Proprietary, etc.]

---

## Support & Contact

For questions, issues, or contributions:

- Open an issue on the GitHub repository
- Contact the development team at [your-email@example.com]

---

## Acknowledgments

- **Unity Technologies** - Unity 2022.3 LTS game engine
- **Cinemachine** - Virtual camera system
- **TextMesh Pro** - Advanced text rendering
- **Newtonsoft.Json** - JSON serialization library
- **Sweet.uTility** - Coroutine management utilities
- **LINQtoGameObject** - GameObject query library
- **Flutter Community** - Flutter-Unity integration support

---

**Last Updated**: October 2025
**Project Version**: 0.1
**Unity Version**: 2022.3.62f2
