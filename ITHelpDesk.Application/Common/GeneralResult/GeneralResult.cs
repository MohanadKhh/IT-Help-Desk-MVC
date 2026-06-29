namespace ITHelpDesk.Application;

public class GeneralResult : IGeneralResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, List<Error>>? Errors { get; set; }

    public static GeneralResult SuccessedResult(string message = "Success") => new()
    {
        Success = true,
        Message = message,
        Errors = null
    };
    public static GeneralResult NotFound(string message = "Resources not found") => new()
    {
        Success = false,
        Message = message,
        Errors = null
    };
    public static GeneralResult FailedResult(string message = "Failed") => new()
    {
        Success = false,
        Message = message,
        Errors = null
    };

    public static GeneralResult FailedResult(Dictionary<string, List<Error>> errors
                , string message = "One or more validation errors occurred") => new()
                {
                    Success = false,
                    Message = message,
                    Errors = errors
                };
}
public class GeneralResult<T> : GeneralResult
{
    public T Data { get; set; } = default!;

    public static GeneralResult<T> SuccessedResult(T data, string message = "Success") => new()
    {
        Success = true,
        Message = message,
        Data = data,
        Errors = null
    };
    public static new GeneralResult<T> NotFound(string message = "Resources not found") => new()
    {
        Success = false,
        Message = message,
        Errors = null
    };
    public static new GeneralResult<T> FailedResult(string message = "Failed") => new()
    {
        Success = false,
        Message = message,
        Errors = null
    };

    public static new GeneralResult<T> FailedResult(Dictionary<string, List<Error>> errors
                , string message = "One or more validation errors occurred") => new()
                {
                    Success = false,
                    Message = message,
                    Errors = errors
                };
}
