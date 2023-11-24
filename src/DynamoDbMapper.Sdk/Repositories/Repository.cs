using System.Globalization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using DynamoDbMapper.Sdk.Entities;
using DynamoDbMapper.Sdk.Extensions;
using DynamoDbMapper.Sdk.Interfaces;

namespace DynamoDbMapper.Sdk.Repositories;

public class Repository<T> : IRepository<T>
    where T : Entity
{
    protected readonly IDynamoDBContext _dynamoDbContext;
    private readonly MultiTenantScoped _multiTenant;
    private readonly IAmazonDynamoDB _amazonDynamoDb;
    protected string _entityType;

    public Repository(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB amazonDynamoDb, MultiTenantScoped multiTenant)
    {
        _dynamoDbContext = dynamoDbContext;
        _amazonDynamoDb = amazonDynamoDb;
        _multiTenant = multiTenant;
        _entityType = typeof(T).Name;
    }
    
    public virtual async Task<T> Save(T entity)
    {
        var oldEntities = (await FindById(entity.Id))?.SegregateEntities() ?? new List<Entity>();
        var entitiesToSave = entity.SegregateEntities();
        
        foreach (var entityToSave in entitiesToSave)
        {
            entityToSave.UpdatedAt = DateTime.Now.ToString("O");
            if (entityToSave is TenantEntity m)
                m.UserId = _multiTenant.UserId;
            
            var item = oldEntities.FirstOrDefault(x => x.Id == entityToSave.Id);
            
            if (!oldEntities.Any() || item is not null)
            {
                await _amazonDynamoDb.PutItemAsync(
                    new PutItemRequest(
                        entityToSave.GetPropertyWithAttribute<DynamoDBTableAttribute>().TableName, 
                        entityToSave.AttributeValues()));
            }
        }

        foreach (var oldEntity in oldEntities)
        {
            var item = entitiesToSave.FirstOrDefault(x => x.Id == oldEntity.Id);
            if (item is null)
            {
                await _amazonDynamoDb.DeleteItemAsync(new DeleteItemRequest(
                    oldEntity.GetPropertyWithAttribute<DynamoDBTableAttribute>().TableName,
                    new Dictionary<string, AttributeValue>
                    {
                        {nameof(Entity.Id),new AttributeValue(oldEntity.Id)} ,
                        {nameof(Entity.CreatedAt),new AttributeValue(oldEntity.CreatedAt.ToString(CultureInfo.InvariantCulture))} 
                    }));
            }
        }

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
            .FindAsync();

    public virtual async Task<Pagination<T>> GetAll() 
        => await CreateQuery()
            .ByGsi(x => x.InheritedType, _entityType)
            .QueryAsync();

    public virtual async Task Remove(T entity) 
        => await _dynamoDbContext.DeleteAsync(entity);
}
