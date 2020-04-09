using System;
using FluentValidation;
using Sporty.Common.Dto.User.Request;
using Sporty.Infra.WebApi.Infrastructure.Helpers;

namespace Sporty.Services.Users.DTO.Request.Validation
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(o => o.FirstName).NotEmpty();
            RuleFor(o => o.LastName).NotEmpty();
            RuleFor(o => o.DateOfBirth)
                .NotEmpty()
                .Must(PropertyValidation.IsValidDateTime)
                .LessThan(DateTime.Today).WithMessage("You cannot enter a birth date in the future.");
        }
    }
}