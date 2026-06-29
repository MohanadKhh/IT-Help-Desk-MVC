using FluentValidation;
using ITHelpDesk.Application.DTOs.TicketComments;

namespace ITHelpDesk.Application.Validators.TicketComments;

public class EditTicketCommentDtoValidator : AbstractValidator<EditTicketCommentDto>
{
    public EditTicketCommentDtoValidator()
    {
        RuleFor(x => x.CommentId)
            .GreaterThan(0).WithMessage("Invalid comment ID.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters.");
    }
}
