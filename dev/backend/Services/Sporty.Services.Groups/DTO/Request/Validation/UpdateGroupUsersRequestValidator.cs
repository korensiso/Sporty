using System;
using FluentValidation;
using Sporty.Common.Dto.Group.Request;

namespace Sporty.Services.Groups.DTO.Request.Validation
{
    public class UpdateGroupUsersRequestValidator : AbstractValidator<UpdateGroupUsersRequest>
    {
        public UpdateGroupUsersRequestValidator()
        {
            RuleFor(o => o.Users).ForEach(id => Guid.Parse(id.ToString()));
        }
    }
}
