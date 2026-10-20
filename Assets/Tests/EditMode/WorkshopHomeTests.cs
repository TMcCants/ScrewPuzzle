using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ScrewPuzzle.Tests
{
    public sealed class WorkshopHomeTests
    {
        [UnityTest]
        public IEnumerator Splash_Profile_Settings_AndCategoryRoundTrip()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.Home);
            yield return new EnterPlayMode();
            yield return null;
            Assert.That(GameObject.Find("Lunyx Logo").GetComponent<RawImage>().texture, Is.Not.Null);
            Assert.That(GameObject.Find("Lunyx Splash").GetComponent<Button>(), Is.Null);
            yield return WaitForSplash();
            Assert.That(GameObject.Find("Lunyx Splash"), Is.Null);
            Assert.That(GameObject.Find("Open Profile"), Is.Null);
            Click("Open Settings");
            Click("Open Profile");
            string original = WorkshopPlayerProfile.Nickname;
            GameObject.Find("Nickname Input").GetComponent<InputField>().text="  Alex  ";
            Click("Save Nickname");
            Assert.That(WorkshopPlayerProfile.Nickname, Is.EqualTo("Alex"));
            Assert.That(GameObject.Find("Settings Page"), Is.Not.Null);
            Click("Settings Home");
            Assert.That(GameObject.Find("Page Subtitle").GetComponent<Text>().text, Does.Contain("Alex"));
            WorkshopPlayerProfile.SaveName(original);
            Click("Open Settings");
            bool music=WorkshopMusic.IsEnabled;
            Click("Settings Music");
            Assert.That(WorkshopMusic.IsEnabled, Is.Not.EqualTo(music));
            Click("Settings Music");
            Click("How To Play");
            Assert.That(GameObject.Find("Guide Text"), Is.Not.Null);
            Click("Guide Settings");
            Click("Settings Home");
            Click("Home Play");
            Click("Open Toy Shop");
            yield return WaitForScene(ThreeDBoardNavigation.Selector);
            Click("Footer Backing");
            yield return WaitForScene(ThreeDBoardNavigation.Home);
            Assert.That(GameObject.Find("Categories Page"), Is.Not.Null);
            Assert.That(GameObject.Find("Lunyx Splash"), Is.Null);
            Click("Categories Home");
            Assert.That(GameObject.Find("Home Play"), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator Splash_CompletesWithoutInput()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.Home);
            yield return new EnterPlayMode();
            yield return null;
            Assert.That(GameObject.Find("Home Layout").GetComponent<CanvasGroup>().interactable, Is.False);
            for(int i=0;i<5;i++) yield return null;
            Assert.That(GameObject.Find("Lunyx Splash"), Is.Not.Null);
            yield return WaitForSplash();
            Assert.That(GameObject.Find("Lunyx Splash"), Is.Null);
            Assert.That(GameObject.Find("Home Play").GetComponent<Button>().interactable, Is.True);
        }

        [Test]
        public void Continue_PrefersLastUnfinishedRun_AndFallsBackAfterClear()
        {
            string oldLast=PlayerPrefs.GetString(WorkshopPlayerProfile.LastBoardKey,"");
            var saved=new string[WorkshopPlayerProfile.Boards.Length];
            for(int i=0;i<saved.Length;i++)
            {
                saved[i]=PlayerPrefs.GetString(WorkshopRunSave.Key(WorkshopPlayerProfile.Boards[i]),"");
                WorkshopRunSave.Clear(WorkshopPlayerProfile.Boards[i]);
            }
            try
            {
                Assert.That(WorkshopPlayerProfile.ContinueBoard, Is.Null);
                WorkshopRunSave.Write(ThreeDBoardProgress.Radio,"test",new[]{0});
                WorkshopRunSave.Write(ThreeDBoardProgress.ToyCar,"test",new[]{1});
                Assert.That(WorkshopPlayerProfile.ContinueBoard, Is.EqualTo(ThreeDBoardProgress.ToyCar));
                WorkshopRunSave.Clear(ThreeDBoardProgress.ToyCar);
                Assert.That(WorkshopPlayerProfile.ContinueBoard, Is.EqualTo(ThreeDBoardProgress.Radio));
            }
            finally
            {
                for(int i=0;i<saved.Length;i++)
                {
                    string key=WorkshopRunSave.Key(WorkshopPlayerProfile.Boards[i]);
                    if(saved[i]=="") PlayerPrefs.DeleteKey(key); else PlayerPrefs.SetString(key,saved[i]);
                }
                PlayerPrefs.SetString(WorkshopPlayerProfile.LastBoardKey,oldLast);
                PlayerPrefs.Save();
            }
        }

        private static IEnumerator WaitForSplash()
        {
            float deadline=Time.realtimeSinceStartup+15;
            while(GameObject.Find("Lunyx Splash")!=null && Time.realtimeSinceStartup<deadline) yield return null;
            Assert.That(GameObject.Find("Lunyx Splash"), Is.Null);
        }
        private static void Click(string name) { GameObject.Find(name).GetComponent<Button>().onClick.Invoke(); }
        private static IEnumerator WaitForScene(string path)
        {
            float deadline=Time.realtimeSinceStartup+10;
            while(SceneManager.GetActiveScene().path!=path && Time.realtimeSinceStartup<deadline) yield return null;
            yield return null;
            Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo(path));
        }
        [UnityTearDown]
        public IEnumerator LeavePlayMode() { if(Application.isPlaying) yield return new ExitPlayMode(); }
    }
}
