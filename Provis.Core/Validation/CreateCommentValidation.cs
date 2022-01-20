using FluentValidation;
using Provis.Core.DTO.CommentsDTO;

namespace Provis.Core.Validation
{
    public class CreateCommentValidation : AbstractValidator<CreateCommentDTO>
    {
        public CreateCommentValidation()
        {
            RuleFor(x => x.TaskId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.WorkspaceId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.CommentText)
                .NotNull()
                .NotEmpty()
                .Length(0, 1000);
        }
    }
}
