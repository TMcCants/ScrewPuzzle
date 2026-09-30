using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ScrewPuzzle.Editor
{
    public static class Radio3DExperimentMenu
    {
        private const string ScenePath = "Assets/Scenes/Experiment_Radio3D.unity";

        [MenuItem("Tools/ScrewPuzzle/Open 3D Radio Test")]
        public static void Open()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(ScenePath);
        }

        [MenuItem("Tools/ScrewPuzzle/Build 3D Radio Android Test")]
        public static void BuildAndroid()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                EditorUtility.DisplayDialog("3D Radio Test",
                    "Switch the project to Android in Build Profiles, then run this command again.", "OK");
                return;
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            string path = EditorUtility.SaveFilePanel("Save 3D Radio test APK", "", "Radio3D-Test", "apk");
            if (string.IsNullOrEmpty(path)) return;

            string originalId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            string originalName = PlayerSettings.productName;
            bool originalBundle = EditorUserBuildSettings.buildAppBundle;
            try
            {
                // Separate app and PlayerPrefs, preserving the installed game's progress.
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, originalId + ".radio3dtest");
                PlayerSettings.productName = "ScrewPuzzle 3D Test";
                EditorUserBuildSettings.buildAppBundle = false;
                BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = new[] { ScenePath },
                    locationPathName = path,
                    target = BuildTarget.Android,
                    options = BuildOptions.Development
                });
                EditorUtility.DisplayDialog("3D Radio Test",
                    report.summary.result == BuildResult.Succeeded
                        ? "Test APK built successfully. Install it on your phone."
                        : "Build did not succeed. See the Unity Console for details.", "OK");
            }
            finally
            {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, originalId);
                PlayerSettings.productName = originalName;
                EditorUserBuildSettings.buildAppBundle = originalBundle;
            }
        }
    }
}
