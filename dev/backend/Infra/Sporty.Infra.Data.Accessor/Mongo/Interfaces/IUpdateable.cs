namespace Sporty.Infra.Data.Accessor.Mongo.Interfaces
{
    public interface IUpdateable<T>
    {
        T Update(T update);
    }
}
