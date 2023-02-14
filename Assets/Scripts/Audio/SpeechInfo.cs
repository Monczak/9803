using System.Collections.Generic;

namespace NineEightOhThree.Audio
{
    public struct SpeechInfo
    {
        public float LengthSeconds { get; init; }
        public int SampleRate { get; init; }
        public int NumSamples { get; init; }
        
        public List<(int start, int end)> WordBoundaries { get; init; } 
    }
}