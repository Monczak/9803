using System.Threading.Tasks;
using SamSharp.Parser;
using SamSharp.Renderer;

namespace SamSharp
{
    public class Sam
    {
        public Options Options { get; set; }

        public Sam(Options options)
        {
            Options = options;
        }

        public Sam() : this(new Options())
        {
        }

        public RenderResult Speak(string input)
        {
            Reciter.Reciter reciter = new Reciter.Reciter();
            return SpeakPhonetic(reciter.TextToPhonemes(input));
        }

        public RenderResult SpeakPhonetic(string phoneticInput)
        {
            Parser.Parser parser = new Parser.Parser();
            Renderer.Renderer renderer = new Renderer.Renderer();

            var data = parser.Parse(phoneticInput);
            return renderer.Render(data, Options);
        }

        public Task<RenderResult> SpeakAsync(string input)
        {
            return Task<RenderResult>.Factory.StartNew(() => Speak(input));
        }

        public Task<RenderResult> SpeakPhoneticAsync(string phoneticInput)
        {
            return Task<RenderResult>.Factory.StartNew(() => SpeakPhonetic(phoneticInput));
        }

        public PhonemeData[] GetPhonemeData(string input)
        {
            return GetPhonemeDataPhonetic(new Reciter.Reciter().TextToPhonemes(input));
        }

        public Task<PhonemeData[]> GetPhonemeDataAsync(string input) =>
            Task<PhonemeData[]>.Factory.StartNew(() => GetPhonemeData(input));

        public PhonemeData[] GetPhonemeDataPhonetic(string phoneticInput) => new Parser.Parser().Parse(phoneticInput);

        public Task<PhonemeData[]> GetPhonemeDataPhoneticAsync(string phoneticInput) =>
            Task<PhonemeData[]>.Factory.StartNew(() => GetPhonemeDataPhonetic(phoneticInput));
    }
}