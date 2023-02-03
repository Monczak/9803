using System;
using System.Threading.Tasks;
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
            sam.SpeakAsync(text).ContinueWith(t => OnSpeechSynthesized(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnSpeechSynthesized(byte[] theAudio)
        {
            AudioClip clip = AudioClip.Create($"speech", theAudio.Length, 1, 22050, false);
            clip.SetData(ByteArrayToFloatArray(theAudio), 0);
            
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
    }
}