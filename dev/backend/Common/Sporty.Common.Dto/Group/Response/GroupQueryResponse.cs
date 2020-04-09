using System;
using System.Collections.Generic;

namespace Sporty.Common.Dto.Group.Response
{
    public class GroupQueryResponse
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public List<Guid> Users { get; set; }
    }
}
