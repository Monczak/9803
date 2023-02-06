using System.Threading.Tasks;
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

        public byte[] Speak(string input)
        {
            Reciter.Reciter reciter = new Reciter.Reciter();
            return SpeakPhonetic(reciter.TextToPhonemes(input));
        }

        public byte[] SpeakPhonetic(string phoneticInput)
        {
            Parser.Parser parser = new Parser.Parser();
            Renderer.Renderer renderer = new Renderer.Renderer();

            var data = parser.Parse(phoneticInput);
            return renderer.Render(data, Options);
        }

        public Task<byte[]> SpeakAsync(string input)
        {
            return Task<byte[]>.Factory.StartNew(() => Speak(input));
        }

        public Task<byte[]> SpeakPhoneticAsync(string phoneticInput)
        {
            return Task<byte[]>.Factory.StartNew(() => SpeakPhonetic(phoneticInput));
        }

        public Parser.Parser.PhonemeData[] GetPhonemeData(string input) => new Parser.Parser().Parse(new Reciter.Reciter().TextToPhonemes(input));

        public Task<Parser.Parser.PhonemeData[]> GetPhonemeDataAsync(string input) =>
            Task<Parser.Parser.PhonemeData[]>.Factory.StartNew(() => GetPhonemeData(input));
    }
}