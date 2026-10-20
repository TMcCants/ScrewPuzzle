using UnityEngine;

namespace ScrewPuzzle
{
    public static class WorkshopPlayerProfile
    {
        public const string NameKey = "ScrewPuzzle.Player.Nickname.v1";
        public const string LastBoardKey = "ScrewPuzzle.Player.LastRun.v1";
        public static readonly string[] Boards = { ThreeDBoardProgress.Radio, ThreeDBoardProgress.ToyCar,
            ThreeDBoardProgress.ToyRobot, ThreeDBoardProgress.ToyTrain, ThreeDBoardProgress.ToyPlane };
        public static string Nickname => PlayerPrefs.GetString(NameKey, "");
        public static void SaveName(string value)
        {
            value = (value ?? "").Trim();
            var clean = new System.Text.StringBuilder();
            foreach (char c in value) if (!char.IsControl(c)) clean.Append(c);
            value = clean.ToString();
            var elements = new System.Globalization.StringInfo(value);
            if (elements.LengthInTextElements > 20) value = elements.SubstringByTextElements(0, 20);
            PlayerPrefs.SetString(NameKey, value); PlayerPrefs.Save();
        }
        public static string ContinueBoard
        {
            get
            {
                string last = PlayerPrefs.GetString(LastBoardKey, "");
                if (System.Array.IndexOf(Boards,last) >= 0 && WorkshopRunSave.HasRun(last)) return last;
                foreach (string board in Boards) if (WorkshopRunSave.HasRun(board)) return board;
                return null;
            }
        }
        public static string BoardName(string id)
        {
            if (id == ThreeDBoardProgress.Radio) return "Radio";
            if (id == ThreeDBoardProgress.ToyCar) return "Race Car";
            if (id == ThreeDBoardProgress.ToyRobot) return "Toy Robot";
            if (id == ThreeDBoardProgress.ToyTrain) return "Toy Train";
            return "Toy Airplane";
        }
        public static void Continue()
        {
            string id = ContinueBoard;
            if (id == ThreeDBoardProgress.Radio) ThreeDBoardNavigation.OpenRadio();
            else if (id == ThreeDBoardProgress.ToyCar) ThreeDBoardNavigation.OpenToyCar();
            else if (id == ThreeDBoardProgress.ToyRobot) ThreeDBoardNavigation.OpenToyRobot();
            else if (id == ThreeDBoardProgress.ToyTrain) ThreeDBoardNavigation.OpenToyTrain();
            else if (id == ThreeDBoardProgress.ToyPlane) ThreeDBoardNavigation.OpenToyPlane();
        }
    }
}
