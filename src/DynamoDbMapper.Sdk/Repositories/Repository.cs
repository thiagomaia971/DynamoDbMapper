using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DynamoDbMapper.Sdk.Entities;
using DynamoDbMapper.Sdk.Interfaces;

namespace DynamoDbMapper.Sdk.Repositories;

public class Repository<T> : IRepository<T>
    where T : Entity
{
    protected readonly IDynamoDBContext _dynamoDbContext;
    private readonly MultiTenantScoped _multiTenant;
    protected string _entityType;

    public Repository(IDynamoDBContext dynamoDbContext, MultiTenantScoped multiTenant)
    {
        _dynamoDbContext = dynamoDbContext;
        _multiTenant = multiTenant;
        _entityType = Activator.CreateInstance<T>().EntityType;
    }

    public virtual async Task<T> Save(T entity)
    {
        if (entity is TenantEntity m)
            m.UserId = _multiTenant.UserId;
        await _dynamoDbContext.SaveAsync(entity);
        return entity;
    }

    public DynamoDbQueryBuilder<T> CreateQuery()
    {
        var multiTenantUserId = typeof(TenantEntity).IsAssignableFrom(typeof(T)) ? _multiTenant.UserId : null;
        return DynamoDbQueryBuilder<T>
            .CreateQuery(_dynamoDbContext, multiTenantUserId);
    }

    public virtual async Task<T> FindById(string id) 
        => await CreateQuery()
            .ById(id)
            .ByInheritedType()
            .FindAsync();

    public virtual async Task<Pagination<T>> GetAll() 
        => await CreateQuery()
            .ByGsi(x => x.InheritedType, _entityType)
            .QueryAsync();

    public virtual async Task Remove(T entity) 
        => await _dynamoDbContext.DeleteAsync(entity);
}