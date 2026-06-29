using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ITHelpDesk.MVC.Helpers;

public static class ModelStateExtensions
{
    /// <summary>
    /// Adds validation errors from a GeneralResult.Errors dictionary into ModelState
    /// so that asp-validation-for tag helpers can display per-field error messages.
    /// </summary>
    /// <param name="modelState">The controller's ModelState.</param>
    /// <param name="errors">Errors dictionary from GeneralResult (keyed by property name).</param>
    /// <param name="prefix">
    /// Optional prefix for the ModelState key.  
    /// Use "Input" when the DTO is a property of a ViewModel (e.g., vm.Input.Title → "Input.Title").
    /// Leave empty/null when the DTO is the model itself (e.g., LoginDto.Email → "Email").
    /// </param>
    public static void AddValidationErrors(
        this ModelStateDictionary modelState,
        Dictionary<string, List<Application.Error>>? errors,
        string? prefix = null)
    {
        if (errors is null) return;

        foreach (var (propertyName, errorList) in errors)
        {
            var key = string.IsNullOrEmpty(prefix)
                ? propertyName
                : $"{prefix}.{propertyName}";

            foreach (var error in errorList)
            {
                modelState.AddModelError(key, error.Message);
            }
        }
    }
}
