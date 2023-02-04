using System;
using System.Threading.Tasks;
using NineEightOhThree.Dialogues;
using UnityEngine;
using SamSharp;
using UnityEditor;

namespace NineEightOhThree.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SpeechManager : MonoBehaviour
    {
        public static SpeechManager Instance { get; private set; }

        private AudioSource source;
        private Sam sam;

        private void Awake()
        {
            Instance ??= this;
            if (Instance != this) Destroy(gameObject);

            source = GetComponent<AudioSource>();
            sam = new Sam();
        }

        public void Speak(string text)
        {
            sam.SpeakAsync(text).ContinueWith(t => OnSpeechSynthesized(t.Result),
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SpeakDialogueLine(DialogueLine line)
        {
            sam.Options = line.Options;
            Speak(line.Text);
        }

        private void OnSpeechSynthesized(byte[] theAudio)
        {
            AudioClip clip = AudioClip.Create($"speech", theAudio.Length, 1, 22050, false);

            float[] data = ByteArrayToFloatArray(theAudio);
            AddFadeout(data);
            clip.SetData(data, 0);
            
            source.PlayOneShot(clip);
        }

        private float[] ByteArrayToFloatArray(byte[] array)
        {
            float[] arr = new float[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                arr[i] = array[i] / 128.0f - 1;
            }

            return arr;
        }

        private void AddFadeout(float[] data, int samples = 1000)
        {
            float x = 0;
            for (int i = data.Length - samples - 1; i < data.Length; i++)
                data[i] *= 1 - x++ / data.Length;
        }
    }
}