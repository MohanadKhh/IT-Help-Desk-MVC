using FluentValidation;
using ITHelpDesk.Application.DTOs.TicketComments;

namespace ITHelpDesk.Application.Validators.TicketComments;

public class CreateTicketCommentDtoValidator : AbstractValidator<CreateTicketCommentDto>
{
    public CreateTicketCommentDtoValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("Invalid ticket ID.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters.");
    }
}
