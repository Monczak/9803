namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class ParsingResult
    {
        public bool Failed { get; }
        public ParserError? TheError { get; init; }
        
        public ParsingResult(bool failed, ParserError? theError)
        {
            Failed = failed;
            TheError = theError;
        }
        
        public static ParsingResult Success() => new(failed: false, theError: null);
        public static ParsingResult Error(ParserError? error) => new(failed: true, theError: error);
    }
    
    public class ParsingResult<T> : ParsingResult
    {
        public T Result { get; }
        
        public ParsingResult(bool failed, ParserError? theError, T result) : base(failed, theError)
        {
            Result = result;
        }
        
        public static ParsingResult<T> Success(T result) => new(failed: false, theError: null, result: result);
        
        public new static ParsingResult<T> Success() => new(failed: false, theError: null, result: default);
        public new static ParsingResult<T> Error(ParserError? error) => new(failed: true, theError: error, result: default);
    }
}