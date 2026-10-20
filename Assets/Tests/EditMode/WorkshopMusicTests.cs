using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace ScrewPuzzle.Tests
{
    public class WorkshopMusicTests : WorkshopRunTestIsolation
    {
        private int previous;
        [SetUp] public void SavePreference() { previous = PlayerPrefs.GetInt(WorkshopMusic.PreferenceKey, -1); }
        [UnityTest] public IEnumerator Music_PersistsAcrossBoards_AndIsIndependentOfEffects()
        {
            PlayerPrefs.SetInt(WorkshopMusic.PreferenceKey, 1);
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.Selector);
            yield return new EnterPlayMode(); yield return null;
            var music = Object.FindFirstObjectByType<WorkshopMusic>();
            Assert.That(music, Is.Not.Null);
            var source = music.GetComponent<AudioSource>();
            Assert.That(source.clip.length, Is.EqualTo(48).Within(.01));
            Assert.That(source.loop, Is.True);
            float[] samples = new float[source.clip.samples];
            source.clip.GetData(samples, 0);
            float peak = 0;
            foreach (float sample in samples) peak = Mathf.Max(peak, Mathf.Abs(sample));
            Assert.That(peak, Is.InRange(.01f, .99f));
            Assert.That(Mathf.Abs(samples[0]-samples[samples.Length-1]), Is.LessThan(.05f));
            bool effects = FeedbackAudio.IsSoundEnabled;
            WorkshopMusic.Toggle();
            Assert.That(WorkshopMusic.IsEnabled, Is.False);
            Assert.That(FeedbackAudio.IsSoundEnabled, Is.EqualTo(effects));
            ThreeDBoardNavigation.OpenToyPlane(); yield return null; yield return null;
            Assert.That(Object.FindObjectsByType<WorkshopMusic>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
            Assert.That(Object.FindFirstObjectByType<WorkshopMusic>(), Is.SameAs(music));
            Assert.That(WorkshopMusic.IsEnabled, Is.False);
            WorkshopMusic.Toggle();
            music.SendMessage("OnApplicationFocus", true);
            music.SendMessage("OnApplicationPause", false);
            yield return new WaitForSecondsRealtime(1);
            Assert.That(source.volume, Is.EqualTo(.14f).Within(.01));
            music.SendMessage("OnApplicationPause", true);
            Assert.That(source.volume, Is.Zero);
            music.SendMessage("OnApplicationPause", false);
            ThreeDBoardNavigation.OpenSelector(); yield return null;
            music.SendMessage("OnApplicationFocus", true);
            yield return new WaitForSecondsRealtime(2);
            Assert.That(source.volume, Is.EqualTo(.28f).Within(.01));
        }
        [UnityTearDown] public IEnumerator Restore()
        {
            if (Application.isPlaying) yield return new ExitPlayMode();
            if (previous < 0) PlayerPrefs.DeleteKey(WorkshopMusic.PreferenceKey);
            else PlayerPrefs.SetInt(WorkshopMusic.PreferenceKey, previous);
            PlayerPrefs.Save();
        }
    }
}
