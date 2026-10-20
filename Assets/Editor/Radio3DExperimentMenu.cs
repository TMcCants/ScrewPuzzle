using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ScrewPuzzle.Editor
{
    public static class Radio3DExperimentMenu
    {
        private const string ScenePath = "Assets/Scenes/Experiment_Radio3D.unity";

        [MenuItem("Tools/ScrewPuzzle/Open 3D Workshop")]
        public static void OpenWorkshop()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(ThreeDBoardNavigation.Home);
        }

        [MenuItem("Tools/ScrewPuzzle/Build 3D Workshop Android Test")]
        public static void BuildWorkshopAndroid()
        {
            BuildAndroidScene(ThreeDBoardNavigation.Home, "3D Workshop", "workshop3dtest");
        }

        [MenuItem("Tools/ScrewPuzzle/Build and Run 3D Workshop on Phone")]
        public static void BuildAndRunWorkshopAndroid()
        {
            BuildAndroidScene(ThreeDBoardNavigation.Home, "3D Workshop", "workshop3dtest", true);
        }

        [MenuItem("Tools/ScrewPuzzle/Open 3D Radio Test")]
        public static void Open()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(ScenePath);
        }

        [MenuItem("Tools/ScrewPuzzle/Build 3D Radio Android Test")]
        public static void BuildAndroid()
        {
            BuildAndroidScene(ScenePath, "3D Radio Test", "radio3dtest");
        }

        [MenuItem("Tools/ScrewPuzzle/Open 3D Toy Car Test")]
        public static void OpenToyCar()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene("Assets/Scenes/Experiment_ToyCar3D.unity");
        }

        [MenuItem("Tools/ScrewPuzzle/Build 3D Toy Car Android Test")]
        public static void BuildToyCarAndroid()
        {
            BuildAndroidScene("Assets/Scenes/Experiment_ToyCar3D.unity", "3D Toy Car Test", "toycar3dtest");
        }

        private static void BuildAndroidScene(string scenePath, string title, string suffix, bool runOnPhone = false)
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                EditorUtility.DisplayDialog(title,
                    "Switch the project to Android in Build Profiles, then run this command again.", "OK");
                return;
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            string path = EditorUtility.SaveFilePanel("Save " + title + " APK", "", suffix, "apk");
            if (string.IsNullOrEmpty(path)) return;

            string originalId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            string originalName = PlayerSettings.productName;
            bool originalBundle = EditorUserBuildSettings.buildAppBundle;
            bool originalScriptsOnly = EditorUserBuildSettings.buildScriptsOnly;
            try
            {
                // Separate app and PlayerPrefs, preserving the installed game's progress.
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, originalId + "." + suffix);
                PlayerSettings.productName = "ScrewPuzzle " + title;
                EditorUserBuildSettings.buildAppBundle = false;
                // A scripts-only build can retain the original game's scene data.
                EditorUserBuildSettings.buildScriptsOnly = false;
                BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = ThreeDBoardNavigation.BuildScenes(scenePath),
                    locationPathName = path,
                    target = BuildTarget.Android,
                    options = BuildOptions.Development | (runOnPhone ? BuildOptions.AutoRunPlayer : BuildOptions.None)
                });
                EditorUtility.DisplayDialog(title,
                    report.summary.result == BuildResult.Succeeded
                        ? (runOnPhone
                            ? "3D Workshop built. Check the connected phone for ScrewPuzzle 3D Workshop. The updated Radio has 18 screws and 6 plates."
                            : "Test APK built successfully. Install this APK and open ScrewPuzzle " + title + ".\n\n" + path)
                        : "Build did not succeed. See the Unity Console for details.", "OK");
            }
            finally
            {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, originalId);
                PlayerSettings.productName = originalName;
                EditorUserBuildSettings.buildAppBundle = originalBundle;
                EditorUserBuildSettings.buildScriptsOnly = originalScriptsOnly;
            }
        }
    }
}
