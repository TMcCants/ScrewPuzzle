using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Provides small runtime-generated interaction sounds for the prototype.
    /// Keeping these cues procedural avoids temporary audio-asset setup while the
    /// final visual and sound direction is still being established.
    /// </summary>
    public sealed class FeedbackAudio : MonoBehaviour
    {
        private const int SampleRate = 44100;

        private static FeedbackAudio instance;

        private AudioSource audioSource;
        private AudioClip selectedClip;
        private AudioClip blockedClip;
        private AudioClip matchClip;

        private static FeedbackAudio Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject audioObject = new GameObject("Feedback Audio");
                    instance = audioObject.AddComponent<FeedbackAudio>();
                    DontDestroyOnLoad(audioObject);
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;

            selectedClip = CreateSelectedClip();
            blockedClip = CreateBlockedClip();
            matchClip = CreateMatchClip();
        }

        public static void PlaySelected()
        {
            Instance.Play(Instance.selectedClip, 0.32f);
        }

        public static void PlayBlocked()
        {
            Instance.Play(Instance.blockedClip, 0.28f);
        }

        public static void PlayMatch()
        {
            Instance.Play(Instance.matchClip, 0.38f);
        }

        private void Play(AudioClip clip, float volume)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip, volume);
            }
        }

        private static AudioClip CreateSelectedClip()
        {
            const float duration = 0.09f;
            int sampleCount = Mathf.CeilToInt(SampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int index = 0; index < sampleCount; index++)
            {
                float time = index / (float)SampleRate;
                float progress = time / duration;
                float frequency = Mathf.Lerp(680f, 430f, progress);
                float envelope = Mathf.Pow(1f - progress, 2f);
                float tone = Mathf.Sin(2f * Mathf.PI * frequency * time);
                float mechanicalClick = index < 80 ? (1f - (index / 80f)) * 0.32f : 0f;
                samples[index] = (tone * 0.58f + mechanicalClick) * envelope;
            }

            return CreateClip("Screw Selected", samples);
        }

        private static AudioClip CreateBlockedClip()
        {
            const float duration = 0.13f;
            int sampleCount = Mathf.CeilToInt(SampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int index = 0; index < sampleCount; index++)
            {
                float time = index / (float)SampleRate;
                float progress = time / duration;
                float envelope = Mathf.Sin(progress * Mathf.PI) * (1f - (progress * 0.45f));
                float wobble = Mathf.Sin(2f * Mathf.PI * 18f * time) * 12f;
                samples[index] = Mathf.Sin(2f * Mathf.PI * (145f + wobble) * time)
                    * envelope
                    * 0.62f;
            }

            return CreateClip("Screw Blocked", samples);
        }

        private static AudioClip CreateMatchClip()
        {
            const float duration = 0.27f;
            int sampleCount = Mathf.CeilToInt(SampleRate * duration);
            float[] samples = new float[sampleCount];
            float[] notes = { 520f, 660f, 820f };

            for (int index = 0; index < sampleCount; index++)
            {
                float time = index / (float)SampleRate;
                float noteDuration = duration / notes.Length;
                int noteIndex = Mathf.Min(Mathf.FloorToInt(time / noteDuration), notes.Length - 1);
                float noteProgress = (time - (noteIndex * noteDuration)) / noteDuration;
                float envelope = Mathf.Sin(noteProgress * Mathf.PI) * (1f - (noteProgress * 0.25f));
                float fundamental = Mathf.Sin(2f * Mathf.PI * notes[noteIndex] * time);
                float overtone = Mathf.Sin(2f * Mathf.PI * notes[noteIndex] * 2f * time) * 0.18f;
                samples[index] = (fundamental + overtone) * envelope * 0.5f;
            }

            return CreateClip("Match Cleared", samples);
        }

        private static AudioClip CreateClip(string clipName, float[] samples)
        {
            AudioClip clip = AudioClip.Create(clipName, samples.Length, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
