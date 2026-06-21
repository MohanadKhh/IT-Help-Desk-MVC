namespace ITHelpDesk.Application;

public interface IGeneralResult
{
    Dictionary<string, List<Error>>? Errors { get; set; }
    string Message { get; set; }
    bool Success { get; set; }

    static abstract GeneralResult FailedResult(string message = "Failed");
    static abstract GeneralResult FailedResult(Dictionary<string, List<Error>> errors, string message = "One or more validation errors occurred");
    static abstract GeneralResult NotFound(string message = "Resources not found");
    static abstract GeneralResult SuccessedResult(string message = "Success");
}
