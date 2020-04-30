using System;

namespace Sporty.Services.Groups.DTO.Response
{
    public class MemberQueryResponse
    {
        public Guid Identifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"Identifier={Identifier}, FirstName={FirstName}, LastName={LastName}";
        }
    }
}
