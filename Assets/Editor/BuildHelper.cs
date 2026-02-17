using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
using System.IO;

/// <summary>
/// iOS TestFlight test build'i kolaylaştıran Editor aracı.
/// Unity menüsünden: Build > iOS Test Build
/// </summary>
public class BuildHelper : EditorWindow
{
    private string buildPath = "Builds/iOS";
    private bool isDevelopmentBuild = true;
    private bool autoconnectProfiler = false;
    private bool deepProfiling = false;

    [MenuItem("Build/iOS Test Build Window")]
    public static void ShowWindow()
    {
        GetWindow<BuildHelper>("iOS Test Build");
    }

    private void OnGUI()
    {
        GUILayout.Label("Hue3D - iOS Test Build", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Version info
        EditorGUILayout.LabelField("Bundle ID", PlayerSettings.applicationIdentifier);
        EditorGUILayout.LabelField("Version", PlayerSettings.bundleVersion);
        EditorGUILayout.LabelField("Build Number", PlayerSettings.iOS.buildNumber);

        GUILayout.Space(10);
        GUILayout.Label("Build Settings", EditorStyles.boldLabel);

        buildPath = EditorGUILayout.TextField("Build Path", buildPath);
        isDevelopmentBuild = EditorGUILayout.Toggle("Development Build", isDevelopmentBuild);
        autoconnectProfiler = EditorGUILayout.Toggle("Autoconnect Profiler", autoconnectProfiler);
        deepProfiling = EditorGUILayout.Toggle("Deep Profiling", deepProfiling);

        GUILayout.Space(10);
        GUILayout.Label("Version Management", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Build Number +1"))
        {
            IncrementBuildNumber();
        }
        if (GUILayout.Button("Minor Version +1"))
        {
            IncrementMinorVersion();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        // Build buttons
        GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
        if (GUILayout.Button("Build iOS (Xcode Project)", GUILayout.Height(40)))
        {
            BuildiOS();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "After build:\n" +
            "1. Open the generated Xcode project on Mac\n" +
            "2. Signing & Capabilities > Select Team\n" +
            "3. Product > Archive\n" +
            "4. Distribute App > TestFlight & App Store\n" +
            "5. Invite test users from App Store Connect",
            MessageType.Info);
    }

    private void IncrementBuildNumber()
    {
        int current = 0;
        int.TryParse(PlayerSettings.iOS.buildNumber, out current);
        current++;
        PlayerSettings.iOS.buildNumber = current.ToString();
        Debug.Log($"[BuildHelper] iOS Build Number: {PlayerSettings.iOS.buildNumber}");
    }

    private void IncrementMinorVersion()
    {
        string version = PlayerSettings.bundleVersion;
        string[] parts = version.Split('.');
        if (parts.Length == 3)
        {
            int minor = 0;
            int.TryParse(parts[1], out minor);
            minor++;
            PlayerSettings.bundleVersion = $"{parts[0]}.{minor}.0";
        }
        Debug.Log($"[BuildHelper] Version: {PlayerSettings.bundleVersion}");
    }

    [MenuItem("Build/iOS Test Build (Quick)")]
    public static void BuildiOSQuick()
    {
        var helper = new BuildHelper();
        helper.isDevelopmentBuild = true;
        helper.buildPath = "Builds/iOS";
        helper.BuildiOS();
    }

    private void BuildiOS()
    {
        // Sahne listesini al
        var scenes = GetEnabledScenes();
        if (scenes.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No active scene in Build Settings!", "OK");
            return;
        }

        // Build klasörünü oluştur
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), buildPath);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        // Build options
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = fullPath,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };

        if (isDevelopmentBuild)
        {
            buildOptions.options |= BuildOptions.Development;
        }
        if (autoconnectProfiler)
        {
            buildOptions.options |= BuildOptions.ConnectWithProfiler;
        }
        if (deepProfiling)
        {
            buildOptions.options |= BuildOptions.EnableDeepProfilingSupport;
        }

        // Build number'ı otomatik artır
        IncrementBuildNumber();

        Debug.Log($"[BuildHelper] iOS Build starting...");
        Debug.Log($"[BuildHelper] Version: {PlayerSettings.bundleVersion}");
        Debug.Log($"[BuildHelper] Build Number: {PlayerSettings.iOS.buildNumber}");
        Debug.Log($"[BuildHelper] Bundle ID: {PlayerSettings.applicationIdentifier}");
        Debug.Log($"[BuildHelper] Path: {fullPath}");

        // Build başlat
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[BuildHelper] iOS Build succeeded! Duration: {summary.totalTime}");
            Debug.Log($"[BuildHelper] Size: {summary.totalSize / (1024 * 1024)} MB");
            EditorUtility.DisplayDialog("Build Succeeded",
                $"iOS Xcode project created!\n\n" +
                $"Location: {fullPath}\n" +
                $"Duration: {summary.totalTime}\n\n" +
                $"Next step: Open in Xcode > Archive > Upload to TestFlight",
                "OK");
            EditorUtility.RevealInFinder(fullPath);
        }
        else
        {
            Debug.LogError($"[BuildHelper] iOS Build failed: {summary.result}");
            EditorUtility.DisplayDialog("Build Failed",
                $"Build error: {summary.result}\n\nCheck the Console.",
                "OK");
        }
    }

    private string[] GetEnabledScenes()
    {
        var scenes = new System.Collections.Generic.List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                scenes.Add(scene.path);
            }
        }
        return scenes.ToArray();
    }
}
