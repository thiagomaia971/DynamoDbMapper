using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DynamoDbMapper.Sdk.Entities;
using DynamoDbMapper.Sdk.Interfaces;

namespace DynamoDbMapper.Sdk.Repositories;

public class Repository<T> : IRepository<T>
    where T : Entity
{
    protected readonly IAmazonDynamoDB _amazonDynamoDb;
    protected readonly IDynamoDBContext _dynamoDbContext;
    private readonly MultiTenantTransient _multiTenant;
    protected DynamoDbQueryBuilder<T> _dynamoDbQueryBuilder;

    public Repository(IAmazonDynamoDB amazonDynamoDb, IDynamoDBContext dynamoDbContext, MultiTenantTransient multiTenant)
    {
        _amazonDynamoDb = amazonDynamoDb;
        _dynamoDbContext = dynamoDbContext;
        _multiTenant = multiTenant;
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
        return _dynamoDbQueryBuilder = DynamoDbQueryBuilder<T>
            .CreateQuery(_amazonDynamoDb, _dynamoDbContext, multiTenantUserId);
    }

    public virtual async Task<T> FindById(string id)
    {
        var entityType = Activator.CreateInstance<T>().EntityType;
        return await CreateQuery()
            .ById(id)
            .ByEntityType(entityType, DynamoDbOperator.BeginsWith)
            .FindAsync();
    }

    public virtual async Task<Pagination<T>> GetAll() 
        => await CreateQuery()
            .ByGsi(Activator.CreateInstance<T>().EntityType, x => x.EntityType, DynamoDbOperator.BeginsWith)
            .ScanAsync();

    public virtual async Task Remove(T entity) 
        => await _dynamoDbContext.DeleteAsync(entity);
}