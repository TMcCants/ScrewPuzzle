using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>Short, scene-owned mechanical cues; respects the game's sound preference.</summary>
    public sealed class Radio3DFeedback : MonoBehaviour
    {
        private AudioSource source;
        private AudioClip unscrew;
        private AudioClip arrival;
        private AudioClip release;

        private void Awake()
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            unscrew = CreateCue("Screw ratchet", 0.36f, 0);
            arrival = CreateCue("Tray click", 0.07f, 1);
            release = CreateCue("Plate release", 0.22f, 2);
        }

        public void PlayUnscrew() { Play(unscrew, 0.23f); }
        public void PlayArrival() { Play(arrival, 0.22f); }
        public void PlayRelease() { Play(release, 0.28f); }
        public void Stop() { if (source != null) source.Stop(); }
        private void Play(AudioClip clip, float volume)
        {
            source.mute = !FeedbackAudio.IsSoundEnabled;
            if (!source.mute) source.PlayOneShot(clip, volume);
        }
        private void Update() { source.mute = !FeedbackAudio.IsSoundEnabled; }
        private void OnApplicationFocus(bool focused) { if (!focused) Stop(); }

        private static AudioClip CreateCue(string name, float duration, int kind)
        {
            const int rate = 44100;
            float[] samples = new float[Mathf.CeilToInt(rate * duration)];
            for (int i = 0; i < samples.Length; i++)
            {
                float t = i / (float)rate;
                float envelope = Mathf.Sin(Mathf.PI * t / duration);
                float value;
                if (kind == 0)
                {
                    float pulse = Mathf.Pow(Mathf.Max(0f, Mathf.Sin(2f * Mathf.PI * 22f * t)), 6f);
                    value = pulse * (0.6f * Mathf.Sin(2f * Mathf.PI * 760f * t) + 0.25f * Mathf.Sin(2f * Mathf.PI * 1730f * t));
                }
                else
                {
                    float frequency = kind == 1 ? 1200f : 185f;
                    value = Mathf.Exp(-t * (kind == 1 ? 60f : 18f)) *
                        (Mathf.Sin(2f * Mathf.PI * frequency * t) + 0.25f * Mathf.Sin(2f * Mathf.PI * frequency * 2.7f * t));
                }
                samples[i] = value * envelope * 0.7f;
            }
            AudioClip clip = AudioClip.Create(name, samples.Length, 1, rate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private void OnDestroy()
        {
            Stop();
            if (unscrew != null) Destroy(unscrew);
            if (arrival != null) Destroy(arrival);
            if (release != null) Destroy(release);
        }
    }
}
