using System;

namespace Sporty.Services.Users.DTO.Response
{
    public class UserQueryResponse
    {
        public Guid Identifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
