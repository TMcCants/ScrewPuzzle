using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    /// <summary>
    /// Owns the level state: playing, won, or lost.
    /// It coordinates systems but does not implement their individual rules.
    /// </summary>
    public sealed class GameManager : MonoBehaviour
    {
        public enum LevelState
        {
            Playing,
            Won,
            Lost
        }

        [SerializeField] private TrayManager trayManager;
        [SerializeField] private RadioRestoration radioRestoration;
        [SerializeField] private Text statusText;
        [SerializeField] private GameObject resultOverlay;
        [SerializeField] private Text resultTitle;
        [SerializeField] private int requiredScrewCount;

        private int removedScrewCount;
        private int clearedScrewCount;
        private Coroutine messageRoutine;

        public LevelState State { get; private set; }
        public bool CanAcceptInput
        {
            get
            {
                return State == LevelState.Playing
                    && trayManager != null
                    && !trayManager.IsBusy;
            }
        }

        private void Start()
        {
            State = LevelState.Playing;
            UpdateStatusText();

            if (resultOverlay != null)
            {
                resultOverlay.SetActive(false);
            }
        }

        public void NotifyScrewRemovedFromObject(Screw screw)
        {
            removedScrewCount++;
            radioRestoration.HandleScrewRemoved(screw);
            UpdateStatusText();
        }

        public void NotifyScrewsCleared(int amount)
        {
            clearedScrewCount += amount;
            UpdateStatusText();
        }

        public void NotifyTrayBecameUnusable()
        {
            if (State != LevelState.Playing)
            {
                return;
            }

            State = LevelState.Lost;
            ShowResult("Tray full!\nTry a different order.");
        }

        public void CheckForWin()
        {
            if (State != LevelState.Playing)
            {
                return;
            }

            bool allRequiredScrewsCleared = clearedScrewCount >= requiredScrewCount;
            bool trayIsEmpty = trayManager.HeldCount == 0;

            if (allRequiredScrewsCleared && trayIsEmpty)
            {
                State = LevelState.Won;
                statusText.text = "Radio restored!";
                radioRestoration.PlayFinalRestoration(OnRestorationFinished);
            }
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void ShowTemporaryMessage(string message)
        {
            if (State != LevelState.Playing || statusText == null)
            {
                return;
            }

            if (messageRoutine != null)
            {
                StopCoroutine(messageRoutine);
            }

            messageRoutine = StartCoroutine(TemporaryMessageRoutine(message));
        }

        public void Configure(
            TrayManager newTrayManager,
            RadioRestoration newRadioRestoration,
            Text newStatusText,
            GameObject newResultOverlay,
            Text newResultTitle,
            int newRequiredScrewCount)
        {
            trayManager = newTrayManager;
            radioRestoration = newRadioRestoration;
            statusText = newStatusText;
            resultOverlay = newResultOverlay;
            resultTitle = newResultTitle;
            requiredScrewCount = newRequiredScrewCount;
        }

        private void OnRestorationFinished()
        {
            ShowResult("RESTORED!\nThe radio is alive.");
        }

        private void ShowResult(string title)
        {
            if (resultTitle != null)
            {
                resultTitle.text = title;
            }

            if (resultOverlay != null)
            {
                resultOverlay.SetActive(true);
            }
        }

        private void UpdateStatusText()
        {
            if (statusText != null)
            {
                statusText.text = "Cleared " + clearedScrewCount + " / " + requiredScrewCount;
            }
        }

        private IEnumerator TemporaryMessageRoutine(string message)
        {
            statusText.text = message;
            yield return new WaitForSeconds(1.1f);
            UpdateStatusText();
            messageRoutine = null;
        }
    }
}
