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
        [SerializeField] private RestorationController restorationController;
        [SerializeField] private Text statusText;
        [SerializeField] private GameObject resultOverlay;
        [SerializeField] private Text resultTitle;
        [SerializeField] private Button resultActionButton;
        [SerializeField] private Text resultActionLabel;
        [SerializeField] private int requiredScrewCount;
        [SerializeField] private int levelNumber;
        [SerializeField] private string nextSceneName;
        [SerializeField] private string winButtonLabel = "CONTINUE";
        [SerializeField] private string restoredStatusMessage = "Object restored!";
        [SerializeField] private string restoredResultMessage = "RESTORED!";

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
            restorationController.HandleScrewRemoved(screw);
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
            SetResultButtonLabel("PLAY AGAIN");
            ShowResult("Game Over!\nThe Tray is full.");
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
                ProgressManager.RecordLevelCompleted(levelNumber);
                statusText.text = restoredStatusMessage;
                restorationController.PlayFinalRestoration(OnRestorationFinished);
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
            RestorationController newRestorationController,
            Text newStatusText,
            GameObject newResultOverlay,
            Text newResultTitle,
            Button newResultActionButton,
            Text newResultActionLabel,
            int newRequiredScrewCount,
            int newLevelNumber,
            string newNextSceneName,
            string newWinButtonLabel,
            string newRestoredStatusMessage,
            string newRestoredResultMessage)
        {
            trayManager = newTrayManager;
            restorationController = newRestorationController;
            statusText = newStatusText;
            resultOverlay = newResultOverlay;
            resultTitle = newResultTitle;
            resultActionButton = newResultActionButton;
            resultActionLabel = newResultActionLabel;
            requiredScrewCount = newRequiredScrewCount;
            levelNumber = newLevelNumber;
            nextSceneName = newNextSceneName;
            winButtonLabel = newWinButtonLabel;
            restoredStatusMessage = newRestoredStatusMessage;
            restoredResultMessage = newRestoredResultMessage;

            if (resultActionButton != null)
            {
                resultActionButton.onClick.AddListener(HandleResultAction);
            }
        }

        private void OnRestorationFinished()
        {
            SetResultButtonLabel(winButtonLabel);
            ShowResult(restoredResultMessage);
        }

        private void HandleResultAction()
        {
            if (State == LevelState.Won && !string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
                return;
            }

            RestartLevel();
        }

        private void SetResultButtonLabel(string label)
        {
            if (resultActionLabel != null)
            {
                resultActionLabel.text = label;
            }
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
