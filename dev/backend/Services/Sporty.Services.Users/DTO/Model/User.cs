using System;
using Sporty.Infra.Data.Accessor.Mongo.Interfaces;
using Sporty.Infra.Data.Accessor.Mongo.Repository;

namespace Sporty.Services.Users.DTO.Model
{
    public class User : Document, IUpdateable<User>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public User Update(User update)
        {
            FirstName = update.FirstName;
            LastName= update.LastName;
            DateOfBirth= update.DateOfBirth;

            return this;
        }
    }
}
