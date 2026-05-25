# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 6000.1.6f1 project using the Universal Mobile 3D Template, specifically configured for mobile 3D game development. The project uses the Universal Render Pipeline (URP) and includes packages optimized for mobile platforms.

## Architecture

- **Unity Version**: 6000.1.6f1 (Unity 6)
- **Template**: Universal Mobile 3D Template
- **Render Pipeline**: Universal Render Pipeline (URP) v17.1.0
- **Target Platforms**: Mobile (iOS/Android focus)
- **Input System**: New Input System v1.14.0

## Key Dependencies

The project includes essential Unity packages:
- `com.unity.render-pipelines.universal`: URP for mobile-optimized rendering
- `com.unity.inputsystem`: New Input System for cross-platform input handling
- `com.unity.feature.mobile`: Mobile-specific features and optimizations
- `com.unity.visualscripting`: Visual scripting support
- `com.unity.timeline`: Timeline and Cinemachine integration

## Development Commands

This is a Unity project managed through the Unity Editor. There are no traditional build scripts or CLI commands. Development workflow:

1. **Open Project**: Open Unity Hub and add/open this project directory
2. **Build**: Use Unity Editor → File → Build Profiles → Build (Unity 6 terminology)
3. **Play/Test**: Use Unity Editor Play button or Ctrl+P (Cmd+P on Mac)
4. **Scripts**: C# scripts are compiled automatically by Unity when saved

### Unity 6 Interface Updates
- **Build Settings** is now called **Build Profiles** in Unity 6000.1.6f1
- Access via: File → Build Profiles (NOT in Window menu)
- Keyboard shortcut: Ctrl+Shift+B (Cmd+Shift+B on Mac)
- **EventSystem**: In Unity 6, EventSystem attaches directly to Canvas components instead of separate GameObject in Hierarchy

### Play Mode Safety Rule - CRITICAL
- **ALWAYS VERIFY PLAY MODE STATUS** before making any changes
- **RED Play button = Play Mode** - ALL changes will be LOST when stopping
- **GREY Play button = Edit Mode** - Changes are permanent
- **MANDATORY CHECK**: Before any component addition, script attachment, or object modification, ALWAYS say: "Stop Play Mode first - any changes in Play Mode will be lost!"
- **Never modify anything while Play button is red**

## Project Structure

- `Assets/`: Main project assets
  - `Scenes/`: Unity scenes (currently contains SampleScene.unity)
  - `Settings/`: Rendering pipeline and quality settings
  - `TutorialInfo/`: Template-specific readme and tutorial scripts
- `ProjectSettings/`: Unity project configuration files
- `Packages/`: Package dependencies defined in manifest.json
- `Library/`: Unity-generated cache (excluded from version control)

## Script Development

- Primary language: C#
- Scripts location: `Assets/` (organize in appropriate subdirectories)
- Unity automatically compiles scripts when saved
- Use Unity's MonoBehaviour for game objects, ScriptableObject for data containers

### Code Quality Standards
- **ZERO COMPILATION WARNINGS POLICY**: No warnings are acceptable in the codebase
- **NO DEPRECATED APIs**: Never use obsolete/deprecated Unity APIs
- Always use the latest recommended APIs for Unity 6000.1.6f1
- Examples of current API replacements:
  - Use `FindFirstObjectByType<T>()` instead of `FindObjectOfType<T>()`
  - Use `Object.FindAnyObjectByType<T>()` if any instance is acceptable
- When in doubt, check Unity 6 documentation for current recommended APIs

### GUID Integrity Policy
- **MANDATORY GUID VERIFICATION**: After ANY code change, script creation, or asset modification, ALWAYS verify and correct GUID references
- **Critical Files to Check**:
  - `ProjectSettings/EditorBuildSettings.asset`: Scene GUIDs must match actual .meta files
  - All `.meta` files: Must have valid 32-character hexadecimal GUIDs
  - Scene files: Component references must use correct script GUIDs
- **Auto-Check Process**: 
  1. Compare scene GUIDs in EditorBuildSettings vs actual .meta GUIDs
  2. Verify all .meta files have properly formatted GUIDs (32 hex chars)
  3. Update any mismatched references immediately
- **Never assume GUIDs are correct** - always verify when touching Unity assets

### Input System Configuration
- **Active Input Handling**: Set to "Both" (`activeInputHandler: 2`) for compatibility
- **EventSystem**: Always use `InputSystemUIInputModule` instead of `StandaloneInputModule`
- This prevents `InvalidOperationException` when using Input System with UGUI

### Performance Configuration
- **Adaptive Performance**: Disabled for this project (simple board game doesn't need automatic optimization)
- Removed from EditorBuildSettings to eliminate console warnings

## Mobile Optimization

This template is pre-configured for mobile development with:
- URP mobile renderer settings
- Mobile-optimized quality settings
- Platform-specific render pipeline assets for mobile vs PC
- Adaptive performance support included