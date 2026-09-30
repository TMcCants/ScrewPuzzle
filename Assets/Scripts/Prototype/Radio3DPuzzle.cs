using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrewPuzzle
{
    public sealed class Radio3DPuzzle : MonoBehaviour
    {
        public enum PuzzleState { Playing, Won, Lost }
        public sealed class Tray
        {
            public bool IsOpen;
            public bool HasColor;
            public ScrewColorId Color;
            public int Count;
        }
        public const int Capacity = 3;
        public readonly Tray[] Trays = { new Tray(), new Tray(), new Tray(), new Tray() };
        public PuzzleState State { get; private set; }
        public bool IsBusy { get; private set; }
        public int RemovedCount { get; private set; }
        public int ClearedCount { get; private set; }
        public int HeldCount { get { return RemovedCount - ClearedCount; } }
        public string Message { get; private set; }
        public bool CanInteract { get { return State == PuzzleState.Playing && !IsBusy; } }
        public System.Func<int, int, Vector3> HoleScreenPosition;
        private readonly Queue<ScrewColorId> jobs = new Queue<ScrewColorId>();
        private Camera view;
        private Radio3DScrew[] screws;
        private Radio3DPlate[] plates;
        public int ReleasedPlateCount
        {
            get
            {
                int count = 0;
                foreach (Radio3DPlate plate in plates) if (plate.IsReleased) count++;
                return count;
            }
        }

        public void Configure(Camera camera, Radio3DScrew[] targets, Radio3DPlate[] panels)
        {
            view = camera;
            screws = targets;
            plates = panels;
            Restart();
        }

        private void AssignNext(Tray tray)
        {
            tray.Count = 0;
            tray.HasColor = jobs.Count > 0;
            if (tray.HasColor) tray.Color = jobs.Dequeue();
        }

        public bool UnlockTestTray(int index)
        {
            if (!CanInteract || index < 2 || index >= Trays.Length || Trays[index].IsOpen) return false;
            if (jobs.Count == 0) { Message = "No more trays needed for this board."; return false; }
            Trays[index].IsOpen = true;
            AssignNext(Trays[index]);
            Message = "Test tray opened. No ad or payment was used.";
            return true;
        }

        public bool TryAdd(Radio3DScrew screw)
        {
            if (!CanInteract || screw == null || screw.IsRemoved) return false;
            int index = System.Array.FindIndex(Trays, t => t.IsOpen && t.HasColor && t.Color == screw.ColorId && t.Count < Capacity);
            if (index < 0)
            {
                Message = "Finish an open tray to make room for " + screw.ColorId.ToString().ToUpperInvariant() + ".";
                return false;
            }
            IsBusy = true;
            Message = "";
            screw.Detach();
            RemovedCount++;
            StartCoroutine(Resolve(screw, index));
            return true;
        }

        private IEnumerator Resolve(Radio3DScrew screw, int index)
        {
            Tray tray = Trays[index];
            Vector3 start = screw.transform.position;
            Quaternion rotation = screw.transform.rotation;
            Quaternion facing = Quaternion.FromToRotation(Vector3.up, -view.transform.forward);
            for (float elapsed = 0; elapsed < 0.28f; elapsed += Time.deltaTime)
            {
                float t = Mathf.SmoothStep(0, 1, elapsed / 0.28f);
                Vector3 screen = HoleScreenPosition != null ? HoleScreenPosition(index, tray.Count) : new Vector3(Screen.width / 2f, Screen.height / 4f);
                screen.z = 5f;
                screw.transform.position = Vector3.Lerp(start, view.ScreenToWorldPoint(screen), t);
                screw.transform.rotation = Quaternion.Slerp(rotation, facing, t);
                screw.transform.localScale = Vector3.one * Mathf.Lerp(1, 0.4f, t);
                yield return null;
            }
            screw.gameObject.SetActive(false);
            tray.Count++;
            if (tray.Count == Capacity)
            {
                float until = Time.time + 0.3f;
                while (Time.time < until) yield return null;
                ClearedCount += Capacity;
                AssignNext(tray);
            }
            foreach (Radio3DPlate plate in plates) yield return plate.ReleaseIfReady();
            if (ClearedCount == screws.Length && ReleasedPlateCount == plates.Length) State = PuzzleState.Won;
            IsBusy = false;
        }

        public void Restart()
        {
            StopAllCoroutines();
            jobs.Clear();
            // Four three-screw orders: every screw has a destination, including without unlocks.
            jobs.Enqueue(ScrewColorId.Red);
            jobs.Enqueue(ScrewColorId.Blue);
            jobs.Enqueue(ScrewColorId.Yellow);
            jobs.Enqueue(ScrewColorId.Red);
            for (int i = 0; i < Trays.Length; i++)
            {
                Trays[i].IsOpen = i < 2;
                Trays[i].HasColor = false;
                Trays[i].Count = 0;
                if (Trays[i].IsOpen) AssignNext(Trays[i]);
            }
            foreach (Radio3DScrew screw in screws) screw.Restore();
            foreach (Radio3DPlate plate in plates) plate.Restore();
            RemovedCount = ClearedCount = 0;
            Message = "";
            State = PuzzleState.Playing;
            IsBusy = false;
        }

        private void OnDestroy()
        {
            if (screws != null)
                foreach (Radio3DScrew screw in screws)
                    if (screw != null && screw.IsRemoved) Destroy(screw.gameObject);
        }
    }
}
