using NineEightOhThree.VirtualCPU.Assembly;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;

namespace NineEightOhThree.VirtualCPU.Assembly
{
    public class OperationResult
    {
        public bool Failed { get; }
        public Error TheError { get; }
        
        public OperationResult(bool failed, Error theError)
        {
            Failed = failed;
            TheError = theError;
        }
        
        public static OperationResult Success() => new(failed: false, theError: null);
        public static OperationResult Error(Error error) => new(failed: true, theError: error);

        public static OperationResult Error(AssemblerError error, Token token)
        {
            if (error == null) throw new InternalErrorException("OperationResult Error was null");
            OperationResult result = new(failed: true, theError: error.WithToken(token));
            return result;
        }
    }
    
    public class OperationResult<T> : OperationResult
    {
        public T Result { get; }
        
        public OperationResult(bool failed, Error theError, T result) : base(failed, theError)
        {
            Result = result;
        }
        
        public static OperationResult<T> Success(T result) => new(failed: false, theError: null, result: result);
        
        public new static OperationResult<T> Success() => new(failed: false, theError: null, result: default);
        public new static OperationResult<T> Error(Error error) => new(failed: true, theError: error, result: default);
        
        public new static OperationResult<T> Error(AssemblerError error, Token token)
        {
            if (error is null) throw new InternalErrorException("OperationResult Error was null");
            OperationResult<T> result = new(failed: true, theError: error.WithToken(token), default);
            return result;
        }
    }
}