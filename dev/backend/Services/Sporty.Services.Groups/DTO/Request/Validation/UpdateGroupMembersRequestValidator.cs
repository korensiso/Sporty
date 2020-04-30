using System;
using FluentValidation;

namespace Sporty.Services.Groups.DTO.Request.Validation
{
    public class UpdateGroupMembersRequestValidator : AbstractValidator<UpdateGroupMembersRequest>
    {
        public UpdateGroupMembersRequestValidator()
        {
            //RuleFor(o => o.Members).ForEach(id => Guid.Parse(id.ToString()));
        }
    }
}
