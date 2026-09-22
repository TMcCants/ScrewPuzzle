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
        private const string TutorialCompletedKey = "ScrewPuzzle.TutorialCompleted";
        private const string OpeningTutorialMessage = "Tap a bright screw to remove it.";
        private const string MatchTutorialMessage = "Match 3 of one color before the tray fills.";

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
        [SerializeField] private Button resultLevelSelectButton;
        [SerializeField] private GameObject navigationOverlay;
        [SerializeField] private GameObject leaveConfirmationOverlay;
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private Text tutorialText;
        [SerializeField] private Button tutorialSkipButton;
        [SerializeField] private int requiredScrewCount;
        [SerializeField] private int levelNumber;
        [SerializeField] private string nextSceneName;
        [SerializeField] private string winButtonLabel = "CONTINUE";
        [SerializeField] private string restoredStatusMessage = "Object restored!";
        [SerializeField] private string restoredResultMessage = "RESTORED!";

        private int removedScrewCount;
        private int clearedScrewCount;
        private Coroutine messageRoutine;
        private Coroutine tutorialMessageRoutine;
        private bool navigationMenuOpen;
        private bool tutorialActive;
        private bool tutorialHasSelectedScrew;
        private string tutorialDefaultMessage = OpeningTutorialMessage;

        public LevelState State { get; private set; }
        public bool CanAcceptInput
        {
            get
            {
                return State == LevelState.Playing
                    && !navigationMenuOpen
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

            if (navigationOverlay != null)
            {
                navigationOverlay.SetActive(false);
            }

            if (leaveConfirmationOverlay != null)
            {
                leaveConfirmationOverlay.SetActive(false);
            }
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape))
            {
                return;
            }

            if (leaveConfirmationOverlay != null && leaveConfirmationOverlay.activeSelf)
            {
                CancelLevelSelect();
                return;
            }

            if (navigationMenuOpen)
            {
                CloseNavigationMenu();
                return;
            }

            if (State == LevelState.Playing)
            {
                OpenNavigationMenu();
                return;
            }

            ReturnToLevelSelect();
        }

        public void NotifyScrewRemovedFromObject(Screw screw)
        {
            removedScrewCount++;
            restorationController.HandleScrewRemoved(screw);

            if (tutorialActive && !tutorialHasSelectedScrew)
            {
                tutorialHasSelectedScrew = true;
                tutorialDefaultMessage = MatchTutorialMessage;
                SetTutorialMessage(tutorialDefaultMessage);
            }

            UpdateStatusText();
        }

        public void NotifyScrewsCleared(int amount)
        {
            clearedScrewCount += amount;

            if (tutorialActive && amount > 0)
            {
                CompleteTutorial();
            }

            UpdateStatusText();
        }

        public void NotifyBlockedScrew()
        {
            if (!tutorialActive || tutorialText == null)
            {
                return;
            }

            if (tutorialMessageRoutine != null)
            {
                StopCoroutine(tutorialMessageRoutine);
            }

            tutorialMessageRoutine = StartCoroutine(BlockedTutorialMessageRoutine());
        }

        public void NotifyTrayBecameUnusable()
        {
            if (State != LevelState.Playing)
            {
                return;
            }

            State = LevelState.Lost;
            FeedbackAudio.PlayLoss();
            SetResultButtonLabel("PLAY AGAIN");
            StartCoroutine(GameOverFeedbackRoutine());
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
                FeedbackAudio.PlayVictory();
                restorationController.PlayFinalRestoration(OnRestorationFinished);
            }
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OpenNavigationMenu()
        {
            if (State != LevelState.Playing || navigationOverlay == null)
            {
                return;
            }

            navigationMenuOpen = true;
            navigationOverlay.SetActive(true);

            if (leaveConfirmationOverlay != null)
            {
                leaveConfirmationOverlay.SetActive(false);
            }
        }

        public void CloseNavigationMenu()
        {
            navigationMenuOpen = false;

            if (navigationOverlay != null)
            {
                navigationOverlay.SetActive(false);
            }

            if (leaveConfirmationOverlay != null)
            {
                leaveConfirmationOverlay.SetActive(false);
            }
        }

        public void RequestLevelSelect()
        {
            if (leaveConfirmationOverlay != null)
            {
                leaveConfirmationOverlay.SetActive(true);
                return;
            }

            ReturnToLevelSelect();
        }

        public void CancelLevelSelect()
        {
            if (leaveConfirmationOverlay != null)
            {
                leaveConfirmationOverlay.SetActive(false);
            }
        }

        public void ReturnToLevelSelect()
        {
            SceneManager.LoadScene("LevelSelect");
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

        public void ConfigureNavigation(
            GameObject newNavigationOverlay,
            GameObject newLeaveConfirmationOverlay,
            Button newResultLevelSelectButton)
        {
            navigationOverlay = newNavigationOverlay;
            leaveConfirmationOverlay = newLeaveConfirmationOverlay;
            resultLevelSelectButton = newResultLevelSelectButton;
        }

        public void ConfigureTutorial(
            GameObject newTutorialPanel,
            Text newTutorialText,
            Button newTutorialSkipButton,
            int tutorialLevelNumber)
        {
            tutorialPanel = newTutorialPanel;
            tutorialText = newTutorialText;
            tutorialSkipButton = newTutorialSkipButton;

            if (tutorialSkipButton != null)
            {
                tutorialSkipButton.onClick.AddListener(SkipTutorial);
            }

            bool shouldShowTutorial =
                tutorialLevelNumber == 1
                && ProgressManager.HighestUnlockedLevel == 1
                && PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 0;

            tutorialActive = shouldShowTutorial;
            tutorialDefaultMessage = OpeningTutorialMessage;
            SetTutorialMessage(tutorialDefaultMessage);

            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(shouldShowTutorial);
            }
        }

        public void SkipTutorial()
        {
            CompleteTutorial();
        }

        private void OnRestorationFinished()
        {
            SetResultButtonLabel(winButtonLabel);
            ShowResult(restoredResultMessage);
        }

        private IEnumerator GameOverFeedbackRoutine()
        {
            if (trayManager != null)
            {
                yield return trayManager.PlayFullTrayFeedback();
            }

            ShowResult("Game Over!\nThe Tray is full.");
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

            if (resultLevelSelectButton != null)
            {
                bool primaryAlreadyReturnsToLevelSelect =
                    State == LevelState.Won && nextSceneName == "LevelSelect";
                resultLevelSelectButton.gameObject.SetActive(!primaryAlreadyReturnsToLevelSelect);
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

        private void SetTutorialMessage(string message)
        {
            if (tutorialText != null)
            {
                tutorialText.text = message;
            }
        }

        private void CompleteTutorial()
        {
            if (!tutorialActive)
            {
                return;
            }

            tutorialActive = false;

            if (tutorialMessageRoutine != null)
            {
                StopCoroutine(tutorialMessageRoutine);
                tutorialMessageRoutine = null;
            }

            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }

            PlayerPrefs.SetInt(TutorialCompletedKey, 1);
            PlayerPrefs.Save();
        }

        private IEnumerator BlockedTutorialMessageRoutine()
        {
            SetTutorialMessage("Dim screws are blocked. Remove the screws holding them first.");
            yield return new WaitForSeconds(2.2f);

            if (tutorialActive)
            {
                SetTutorialMessage(tutorialDefaultMessage);
            }

            tutorialMessageRoutine = null;
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
