using FluentValidation;
using ITHelpDesk.Application.DTOs.Tickets;

namespace ITHelpDesk.Application.Validators.Tickets;

public class EditTicketDtoValidator : AbstractValidator<EditTicketDto>
{
    private static readonly string[] ValidPriorities = ["Low", "Medium", "High", "Critical"];

    public EditTicketDtoValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("Invalid ticket ID.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority is required.")
            .Must(p => ValidPriorities.Contains(p))
            .WithMessage("Priority must be one of: Low, Medium, High, Critical.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Please select a category.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid ticket status.");
    }
}
