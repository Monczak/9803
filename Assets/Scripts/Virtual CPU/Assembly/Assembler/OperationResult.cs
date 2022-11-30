namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class OperationResult
    {
        public bool Failed { get; }
        public AssemblerError? TheError { get; init; }
        
        public OperationResult(bool failed, AssemblerError? theError)
        {
            Failed = failed;
            TheError = theError;
        }
        
        public static OperationResult Success() => new(failed: false, theError: null);
        public static OperationResult Error(AssemblerError? error) => new(failed: true, theError: error);
    }
    
    public class OperationResult<T> : OperationResult
    {
        public T Result { get; }
        
        public OperationResult(bool failed, AssemblerError? theError, T result) : base(failed, theError)
        {
            Result = result;
        }
        
        public static OperationResult<T> Success(T result) => new(failed: false, theError: null, result: result);
        
        public new static OperationResult<T> Success() => new(failed: false, theError: null, result: default);
        public new static OperationResult<T> Error(AssemblerError? error) => new(failed: true, theError: error, result: default);
    }
}