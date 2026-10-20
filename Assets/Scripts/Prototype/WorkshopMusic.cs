using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScrewPuzzle
{
    /// <summary>One continuous, original Toy Shop score, independent of effect volume.</summary>
    public sealed class WorkshopMusic : MonoBehaviour
    {
        public const string PreferenceKey = "ScrewPuzzle.Workshop3D.Music.v1";
        public static bool IsEnabled => PlayerPrefs.GetInt(PreferenceKey, 1) != 0;
        private static WorkshopMusic instance;
        private AudioSource source;
        private AudioClip clip;
        private bool paused;
        private bool focused = true;
        public static void EnsurePlaying()
        {
            if (instance == null) new GameObject("Workshop Music").AddComponent<WorkshopMusic>();
        }
        public static void Toggle()
        {
            PlayerPrefs.SetInt(PreferenceKey, IsEnabled ? 0 : 1);
            PlayerPrefs.Save();
        }
        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.spatialBlend = 0;
            source.volume = 0;
            clip = Compose();
            source.clip = clip;
            source.Play();
        }
        private void Update()
        {
            string scene = SceneManager.GetActiveScene().path;
            bool shop = scene == ThreeDBoardNavigation.Selector || scene == ThreeDBoardNavigation.Home;
            bool workshop = shop || scene == ThreeDBoardNavigation.Radio || scene == ThreeDBoardNavigation.ToyCar || scene == ThreeDBoardNavigation.ToyRobot || scene == ThreeDBoardNavigation.ToyTrain || scene == ThreeDBoardNavigation.ToyPlane;
            float target = IsEnabled && workshop && !paused && focused ? (shop ? .28f : .14f) : 0;
            source.volume = Mathf.MoveTowards(source.volume, target, Time.unscaledDeltaTime * .2f);
        }
        private void OnApplicationPause(bool value) { paused = value; RefreshPause(); }
        private void OnApplicationFocus(bool value) { focused = value; RefreshPause(); }
        private void RefreshPause()
        {
            if (source == null) return;
            if (paused || !focused) { source.Pause(); source.volume = 0; }
            else source.UnPause();
        }
        private void OnDestroy()
        {
            if (instance != this) return;
            instance = null;
            if (clip != null) Destroy(clip);
        }
        private static AudioClip Compose()
        {
            const int rate = 22050;
            const float beat = .75f;
            // Sixteen bars in C major, with note tails wrapped across the loop seam.
            float[] samples = new float[rate * 48];
            int[,] chords = { {48,52,55}, {45,48,52}, {53,57,60}, {43,47,50} };
            int[] melody = {72,76,79,76,74,72,69,72,77,76,74,72,71,67,74,71};
            for (int bar = 0; bar < 16; bar++)
            {
                float start = bar * 4 * beat;
                for (int b = 0; b < 4; b++)
                {
                    AddNote(samples, rate, start + b * beat, chords[bar % 4,b % 3] + 12, 1.4f, .16f, false);
                    if (b % 2 == 0) AddNote(samples, rate, start + b * beat, chords[bar % 4,0], 1.8f, .13f, false);
                }
                AddNote(samples, rate, start + .5f * beat, melody[bar], 2.3f, .12f, true);
                if (bar % 2 == 0) AddNote(samples, rate, start + 2.5f * beat, melody[(bar+1)%16], 2.3f, .09f, true);
            }
            var result = AudioClip.Create("Toy Shop — Little Clockwork", samples.Length, 1, rate, false);
            result.SetData(samples, 0);
            return result;
        }
        private static void AddNote(float[] samples, int rate, float start, int midi, float duration, float gain, bool bell)
        {
            float frequency = 440f * Mathf.Pow(2f, (midi - 69) / 12f);
            int offset = Mathf.RoundToInt(start * rate);
            for (int i = 0; i < duration * rate; i++)
            {
                float t = (float)i / rate;
                float envelope = Mathf.Min(1, t * 100) * Mathf.Exp(-t * (bell ? 2.8f : 4f)) * Mathf.Clamp01((duration-t)*20);
                float phase = 2 * Mathf.PI * frequency * t;
                float tone = Mathf.Sin(phase) + (bell ? .25f : .18f) * Mathf.Sin(phase * (bell ? 2.01f : 2f)) + .06f * Mathf.Sin(phase*3);
                samples[(offset+i)%samples.Length] += tone * envelope * gain;
            }
        }
    }
}
