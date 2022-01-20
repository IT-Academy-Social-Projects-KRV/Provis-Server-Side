using FluentValidation;
using Provis.Core.DTO.CommentsDTO;

namespace Provis.Core.Validation
{
    public class EditCommentValidation : AbstractValidator<CommentEditDTO>
    {
        public EditCommentValidation()
        {
            RuleFor(x => x.CommentId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.WorkspaceId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.CommentText)
                .NotNull()
                .NotEmpty()
                .Length(0, 500);
        }
    }
}
