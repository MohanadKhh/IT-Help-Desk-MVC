using FluentValidation.Results;

namespace ITHelpDesk.Application;

public static class ErrorMapper
{
    public static Dictionary<string, List<Error>> ToError(this ValidationResult validation)
    {
        return validation.Errors.GroupBy(p => p.PropertyName)
            .ToDictionary
            (
                e => e.Key,
                e => e.Select(e => new Error
                {
                    Code = e.ErrorCode,
                    Message = e.ErrorMessage
                }).ToList()
            );
    }
}
