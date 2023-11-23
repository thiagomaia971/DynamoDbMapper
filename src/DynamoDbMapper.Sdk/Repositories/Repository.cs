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
        _entityType = Activator.CreateInstance<T>().EntityType;
    }

    public virtual async Task<T> Save(T entity)
    {
        var entities = entity.SegregateEntities();
        
        foreach (var entityInner in entities)
        {
            if (entityInner is TenantEntity m)
                m.UserId = _multiTenant.UserId;

            var dic = new Dictionary<string, AttributeValue>();
            var properties = entityInner.GetPropertiesWithoutAttribute<DynamoDBIgnoreAttribute>();

            foreach (var property in properties)
            {
                var value = property.GetValue(entityInner);
                if (value is null)
                    continue;
                
                dic.Add(property.GetCollumnName(), new AttributeValue(value.ToString()));
            }
            
            var tableName = entityInner.GetPropertyWithAttribute<DynamoDBTableAttribute>().TableName;
            await _amazonDynamoDb.PutItemAsync(new PutItemRequest(tableName, dic));
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
            .ByInheritedType()
            .FindAsync();

    public virtual async Task<Pagination<T>> GetAll() 
        => await CreateQuery()
            .ByGsi(x => x.InheritedType, _entityType)
            .QueryAsync();

    public virtual async Task Remove(T entity) 
        => await _dynamoDbContext.DeleteAsync(entity);
}

public class Foo {
    public void M(int i) { }
    public void M(string s) { }
    public void M<T>(T t) { }
    public void M<T>(T t, string s) { }
}