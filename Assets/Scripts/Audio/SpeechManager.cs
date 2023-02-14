﻿using System;
using System.Threading.Tasks;
using NineEightOhThree.Dialogues;
using NineEightOhThree.Threading;
using UnityEngine;
using SamSharp;
using SamSharp.Parser;
using SamSharp.Renderer;

namespace NineEightOhThree.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SpeechManager : MonoBehaviour
    {
        public static SpeechManager Instance { get; private set; }

        private AudioSource source;
        private Sam sam;

        private AudioClip clip;

        public event EventHandler<SpeechInfo> OnSpeechSynthesized; 

        private void Awake()
        {
            Instance ??= this;
            if (Instance != this) Destroy(gameObject);

            source = GetComponent<AudioSource>();
            sam = new Sam();
        }

        public async Task SpeakAsync(string text)
        {
            RenderResult result = await sam.SpeakAsync(text);
            await UnityDispatcher.Instance.Execute(() => OnSamDone(result));
        }

        public async Task SpeakDialogueLineAsync(DialogueLine line)
        {
            SetSamOptions(line.Options);
            await SpeakAsync(line.Text);
        }

        public PhonemeData[] GetPhonemeData(string text)
        {
            return sam.GetPhonemeData(text);
        }

        public Task<PhonemeData[]> GetPhonemeDataAsync(string text)
        {
            return sam.GetPhonemeDataAsync(text);
        }

        public void SetSamOptions(Options options) => sam.Options = options;

        private void OnSamDone(RenderResult result)
        {
            if (clip is not null) Destroy(clip);
            clip = AudioClip.Create("speech", result.Audio.Length, 1, 22050, false);

            float[] data = ByteArrayToFloatArray(result.Audio);
            AddFadeout(data);
            clip.SetData(data, 0);
            
            OnSpeechSynthesized?.Invoke(this, new SpeechInfo
            {
                LengthSeconds = clip.length, 
                SampleRate = 22050,
                NumSamples = data.Length,
                WordBoundaries = result.WordBoundaries
            });
            
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