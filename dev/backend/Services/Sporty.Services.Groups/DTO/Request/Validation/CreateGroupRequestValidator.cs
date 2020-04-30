using FluentValidation;

namespace Sporty.Services.Groups.DTO.Request.Validation
{
    internal class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
    {
        public CreateGroupRequestValidator()
        {
            RuleFor(o => o.Name).MaximumLength(15).NotEmpty();
        }
    }
}