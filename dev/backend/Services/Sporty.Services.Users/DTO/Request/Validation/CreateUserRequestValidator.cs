using FluentValidation;
using Sporty.Common.Dto.User.Request;

namespace Sporty.Services.Users.DTO.Request.Validation
{
    internal class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(o => o.FirstName).NotEmpty();
            RuleFor(o => o.LastName).NotEmpty();
            RuleFor(o => o.DateOfBirth).NotEmpty();
        }
    }
}