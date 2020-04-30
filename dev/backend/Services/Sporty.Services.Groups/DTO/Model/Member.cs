using System;
using Sporty.Infra.Data.Accessor.Mongo.Interfaces;
using Sporty.Infra.Data.Accessor.Mongo.Repository;

namespace Sporty.Services.Groups.DTO.Model
{
    public class Member : Document, IUpdateable<Member>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Member Update(Member update)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{base.ToString()} FirstName={FirstName}, LastName={LastName}";
        }
    }
}
