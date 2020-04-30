using System;
using System.Collections.Generic;

namespace Sporty.Services.Groups.DTO.Request
{
    public class UpdateGroupMembersRequest
    {
        public List<Guid> Members { get; set; }
    }
}
