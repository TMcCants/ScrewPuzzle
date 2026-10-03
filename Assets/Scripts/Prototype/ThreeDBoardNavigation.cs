using UnityEngine.SceneManagement;

namespace ScrewPuzzle
{
    public static class ThreeDBoardNavigation
    {
        public const string Selector = "Assets/Scenes/Experiment_3DBoardSelect.unity";
        public const string Radio = "Assets/Scenes/Experiment_Radio3D.unity";
        public const string ToyCar = "Assets/Scenes/Experiment_ToyCar3D.unity";

        public const string ToyRobot = "Assets/Scenes/Experiment_ToyRobot3D.unity";
        public static void OpenToyRobot() { Open(ToyRobot); }
        public static void OpenSelector() { Open(Selector); }
        public static void OpenRadio() { Open(Radio); }
        public static void OpenToyCar() { Open(ToyCar); }

        public static string[] BuildScenes(string first)
        {
            if (first == Radio) return new[] { Radio, Selector, ToyCar, ToyRobot };
            if (first == ToyCar) return new[] { ToyCar, Selector, Radio, ToyRobot };
            if (first == ToyRobot) return new[] { ToyRobot, Selector, Radio, ToyCar };
            return new[] { Selector, Radio, ToyCar, ToyRobot };
        }

        private static void Open(string path)
        {
            // Experiment scenes deliberately remain outside the production game's build list.
#if UNITY_EDITOR
            if (!UnityEngine.Application.CanStreamedLevelBeLoaded(path))
            {
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(path, new LoadSceneParameters(LoadSceneMode.Single));
                return;
            }
#endif
            SceneManager.LoadScene(path, LoadSceneMode.Single);
        }
    }
}
