using UnityEngine.SceneManagement;

namespace ScrewPuzzle
{
    public static class ThreeDBoardNavigation
    {
        public const string Home = "Assets/Scenes/Workshop_Home.unity";
        public static void OpenHome() { WorkshopHomeBootstrap.CategoriesOnArrival=false; Open(Home); }
        public static void OpenCategories() { WorkshopHomeBootstrap.CategoriesOnArrival=true; Open(Home); }
        public const string Selector = "Assets/Scenes/Experiment_3DBoardSelect.unity";
        public const string Radio = "Assets/Scenes/Experiment_Radio3D.unity";
        public const string ToyCar = "Assets/Scenes/Experiment_ToyCar3D.unity";

        public const string ToyPlane = "Assets/Scenes/Experiment_ToyPlane3D.unity";
        public static void OpenToyPlane() { Open(ToyPlane); }
        public const string ToyTrain = "Assets/Scenes/Experiment_ToyTrain3D.unity";
        public static void OpenToyTrain() { Open(ToyTrain); }
        public const string ToyRobot = "Assets/Scenes/Experiment_ToyRobot3D.unity";
        public static void OpenToyRobot() { Open(ToyRobot); }
        public static void OpenSelector() { Open(Selector); }
        public static void OpenRadio() { Open(Radio); }
        public static void OpenToyCar() { Open(ToyCar); }
        public static string NextScene(string boardId)
        {
            if (boardId == ThreeDBoardProgress.Radio) return ToyCar;
            if (boardId == ThreeDBoardProgress.ToyCar) return ToyRobot;
            if (boardId == ThreeDBoardProgress.ToyRobot) return ToyTrain;
            if (boardId == ThreeDBoardProgress.ToyTrain) return ToyPlane;
            return Radio;
        }
        public static void Replay(string board)
        {
            WorkshopRunSave.Clear(board);
            string path=board==ThreeDBoardProgress.Radio ? Radio : board==ThreeDBoardProgress.ToyCar ? ToyCar :
                board==ThreeDBoardProgress.ToyRobot ? ToyRobot : board==ThreeDBoardProgress.ToyTrain ? ToyTrain : ToyPlane;
            Open(path);
        }
        public static void OpenNext(string boardId) { Open(NextScene(boardId)); }

        public static string[] BuildScenes(string first)
        {
            var all = new[] { Home, Selector, Radio, ToyCar, ToyRobot, ToyTrain, ToyPlane };
            var scenes = new System.Collections.Generic.List<string>();
            scenes.Add(System.Array.IndexOf(all, first) >= 0 ? first : Home);
            foreach (string scene in all) if (scene != scenes[0]) scenes.Add(scene);
            return scenes.ToArray();
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
