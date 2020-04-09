using System;
using System.Collections.Generic;

namespace Sporty.Common.Dto.Group.Request
{
    public class UpdateGroupUsersRequest
    {
        public List<Guid> Users { get; set; }
    }
}
