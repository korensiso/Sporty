using FluentValidation;

namespace Sporty.Services.Groups.DTO.Request.Validation
{
    public class UpdateGroupRequestValidator : AbstractValidator<UpdateGroupRequest>
    {
        public UpdateGroupRequestValidator()
        {
            RuleFor(o => o.Name).MaximumLength(15).NotEmpty();
        }


    }
}