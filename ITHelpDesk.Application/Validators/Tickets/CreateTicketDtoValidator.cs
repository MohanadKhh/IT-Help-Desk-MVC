using FluentValidation;
using ITHelpDesk.Application.DTOs.Tickets;

namespace ITHelpDesk.Application.Validators.Tickets;

public class CreateTicketDtoValidator : AbstractValidator<CreateTicketDto>
{
    private static readonly string[] ValidPriorities = ["Low", "Medium", "High", "Critical"];

    public CreateTicketDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters long.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority is required.")
            .Must(p => ValidPriorities.Contains(p))
            .WithMessage("Priority must be one of: Low, Medium, High, Critical.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Please select a category.");
    }
}
