using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NineEightOhThree.Utilities;
using SamSharp.Parser;

namespace SamSharp.Renderer
{
    public partial class Renderer
    {
        private class Formants
        {
            public Dictionary<int, int> Mouth { get; set; }
            public Dictionary<int, int> Throat { get; set; }
            public Dictionary<int, int> Formant3 { get; set; }

            public Formants()
            {
                Mouth = new Dictionary<int, int>();
                Throat = new Dictionary<int, int>();
                Formant3 = new Dictionary<int, int>();
            }
        }
        
                
        private class FramesData
        {
            public Dictionary<int, int> Pitches { get; }
            public Formants Frequencies { get; }
            public Formants Amplitudes { get; }
            public Dictionary<int, int> SampledConsonantFlags { get; }
            
            public HashSet<int> PhonemeStarts { get; }
            public HashSet<int> WordStarts { get; }
            public HashSet<int> WordEnds { get; }
            
            public int T { get; set; }

            public FramesData()
            {
                Pitches = new Dictionary<int, int>();
                Frequencies = new Formants();
                Amplitudes = new Formants();
                SampledConsonantFlags = new Dictionary<int, int>();
                PhonemeStarts = new HashSet<int>();
                WordStarts = new HashSet<int>();
                WordEnds = new HashSet<int>();
            }
        }

        /// <summary>
        /// Renders audio from an array of phoneme data. 
        /// </summary>
        /// <param name="phonemes">The phoneme data output by the parser.</param>
        /// <param name="options">Speech options such as pitch, mouth/throat, speed and sing mode.</param>
        /// <returns>A byte buffer with audio data and the number of samples in the buffer if rendered without the speed modifier.</returns>
        public RenderResult Render(PhonemeData[] phonemes, Options options)
        {
            var sentences = PrepareFrames(phonemes, options);

            int bufSize = (int)(176.4f // 22050 / 125
                                * phonemes.Sum(data => data.Length!.Value)
                                * options.Speed
                                * CurveUtils.IntegrateQuantize(options.SpeedModifier, 0, 1, 1000, options.Speed));

            var output = new OutputBuffer(bufSize);

            PrintOutput(sentences);
            
            var boundaries = ProcessFrames(output, sentences.T, options.Speed, options.SpeedModifier, sentences);

            return new RenderResult {Audio = output.Get(), WordBoundaries = boundaries};
        }

        private void PrintOutput(FramesData framesData)
        {
            Debug.WriteLine("===============================================");
            Debug.WriteLine("Final data for speech output:");
            Debug.WriteLine("flags ampl1 freq1 ampl2 freq2 ampl3 freq3 pitch");
            Debug.WriteLine("-----------------------------------------------");
            for (int i = 0; i < framesData.SampledConsonantFlags.Count; i++)
            {
                Debug.WriteLine($" {framesData.SampledConsonantFlags[i].ToString().PadLeft(5, '0')}" +
                                $" {framesData.Amplitudes.Mouth[i].ToString().PadLeft(5, '0')}" +
                                $" {framesData.Frequencies.Mouth[i].ToString().PadLeft(5, '0')}" +
                                $" {framesData.Amplitudes.Throat[i].ToString().PadLeft(5, '0')}" +
                                $" {framesData.Frequencies.Throat[i].ToString().PadLeft(5, '0')}" +
                                $" {framesData.Amplitudes.Formant3[i].ToString().PadLeft(5, '0')}" +
                                $" {framesData.Frequencies.Formant3[i].ToString().PadLeft(5, '0')}" +
                                $" {framesData.Pitches[i].ToString().PadLeft(5, '0')}");
            }
            Debug.WriteLine("===============================================");
        }
    }

    public struct RenderResult
    {
        public byte[] Audio { get; init; }

        public List<(int start, int end)> WordBoundaries { get; init; }
    }
}