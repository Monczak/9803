using System.Threading;
using System.Threading.Tasks;
using SamSharp.Parser;
using SamSharp.Renderer;

namespace SamSharp
{
    public class Sam
    {
        public Options Options { get; set; }

        private readonly Reciter.Reciter reciter;
        private readonly Parser.Parser parser;
        private readonly Renderer.Renderer renderer;

        public Sam(Options options)
        {
            Options = options;
            
            reciter = new Reciter.Reciter();
            parser = new Parser.Parser();
            renderer = new Renderer.Renderer();
        }

        public Sam() : this(new Options())
        {
        }

        public string TextToPhonemes(string input)
        {
            return reciter.TextToPhonemes(input);
        }

        public RenderResult Speak(string input)
        {
            return SpeakPhonetic(reciter.TextToPhonemes(input));
        }

        public RenderResult SpeakPhonetic(string phoneticInput)
        {
            var data = parser.Parse(phoneticInput);
            return renderer.Render(data, Options);
        }

        public Task<RenderResult> SpeakAsync(string input)
        {
            return Task<RenderResult>.Factory.StartNew(() => Speak(input));
        }

        public Task<RenderResult> SpeakPhoneticAsync(string phoneticInput)
        {
            return Task<RenderResult>.Factory.StartNew(() => SpeakPhonetic(phoneticInput), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }

        public PhonemeData[] GetPhonemeData(string input)
        {
            return GetPhonemeDataPhonetic(new Reciter.Reciter().TextToPhonemes(input));
        }

        public Task<PhonemeData[]> GetPhonemeDataAsync(string input) =>
            Task<PhonemeData[]>.Factory.StartNew(() => GetPhonemeData(input), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

        public PhonemeData[] GetPhonemeDataPhonetic(string phoneticInput) => new Parser.Parser().Parse(phoneticInput);

        public Task<PhonemeData[]> GetPhonemeDataPhoneticAsync(string phoneticInput) =>
            Task<PhonemeData[]>.Factory.StartNew(() => GetPhonemeDataPhonetic(phoneticInput), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
    }
}