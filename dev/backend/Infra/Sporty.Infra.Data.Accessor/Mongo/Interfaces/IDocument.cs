using System;

namespace Sporty.Infra.Data.Accessor.Mongo.Interfaces
{
    /// <summary>
    /// Mongo base record
    /// </summary>
    public interface IDocument<TIdentifier>
    {
        TIdentifier Identifier { get; set; }
        DateTime CreatedOn { get; }
    }
}
