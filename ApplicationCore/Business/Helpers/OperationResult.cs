using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApplicationCore.Business.Helpers;

public class OperationResult<T>
{
    public OperationResult(bool isSuccess, string message, T data = default!)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
    }

    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }

    public static OperationResult<T> Success(string message, T data = default!)
    {
        return new OperationResult<T>(true, message, data);
    }

    public static OperationResult<T> Failure(string message)
    {
        return new OperationResult<T>(false, message);
    }
}


