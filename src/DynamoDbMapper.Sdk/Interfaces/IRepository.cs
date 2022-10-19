using DynamoDbMapper.Sdk.Entities;

namespace DynamoDbMapper.Sdk.Interfaces;

public interface IRepository<T>
    where T : Entity
{
    
    Task<T> Save(T entity);
    DynamoDbQueryBuilder<T> CreateQuery();
    Task<T> FindById(string id);
    Task<Pagination<T>> GetAll();
    Task Remove(T entity);
}