using System;
using System.Collections.Generic;

namespace Sporty.Services.Groups.DTO.Response
{
    public class GroupQueryResponse
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public IEnumerable<Guid> Members { get; set; }
    }
}
