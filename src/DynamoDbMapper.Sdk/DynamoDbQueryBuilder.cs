using System.Linq.Expressions;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DynamoDbMapper.Sdk.Attributes;
using DynamoDbMapper.Sdk.Entities;
using DynamoDbMapper.Sdk.Extensions;

namespace DynamoDbMapper.Sdk;

public class DynamoDbQueryBuilder<T>
    where T: Entity
{
    private int _pageSize = 10;
    private IDynamoDBContext _dynamoDbContext;
    private readonly string _multiTenantUserId;
    private string _entityType;

    private string GsiPropertyName { get; set; }
     private string GsiName => string.IsNullOrEmpty(GsiPropertyName) ? null : $"{GsiPropertyName}";
    private IList<DynamoDbCondition> _conditions;

    private DynamoDbQueryBuilder(
        IDynamoDBContext dynamoDbContext,
        string multiTenantUserId)
    {
        _dynamoDbContext = dynamoDbContext;
        _multiTenantUserId = multiTenantUserId;
        _conditions = new List<DynamoDbCondition>();
        _entityType = Activator.CreateInstance<T>().EntityType;
    }

    public static DynamoDbQueryBuilder<T> CreateQuery(
        IDynamoDBContext dynamoDbContext, 
        string multiTenantUserId) 
        => new(dynamoDbContext, multiTenantUserId);

    public DynamoDbQueryBuilder<T> ById(string value)
    {
        _conditions.Add(DynamoDbCondition.Create("Id", DynamoDbOperator.Equal, value));
        return this;
    }

    public DynamoDbQueryBuilder<T> ByHash(string value, DynamoDbOperator queryOperator = DynamoDbOperator.Equal)
    {
        _conditions.Add(DynamoDbCondition.Create("Hash", queryOperator, value));
        return this;
    }

    public DynamoDbQueryBuilder<T> ByProperty(Expression<Func<T, string>> property, string value, DynamoDbOperator queryOperator = DynamoDbOperator.Equal)
    {
        _conditions.Add(DynamoDbCondition.Create(GetPropertyName(property), queryOperator, value));
        return this;
    }

    public DynamoDbQueryBuilder<T> ByProperty(string property, string value, DynamoDbOperator queryOperator = DynamoDbOperator.Equal)
    {
        _conditions.Add(DynamoDbCondition.Create(property, queryOperator, value));
        return this;
    }

    public DynamoDbQueryBuilder<T> ByEntityType(DynamoDbOperator queryOperator = DynamoDbOperator.Equal)
    {
        _conditions.Add(DynamoDbCondition.Create("EntityType", queryOperator, _entityType));
        return this;
    }

    public DynamoDbQueryBuilder<T> ByInheritedType(DynamoDbOperator queryOperator = DynamoDbOperator.Equal)
    {
        _conditions.Add(DynamoDbCondition.Create("InheritedType", queryOperator, _entityType));
        return this;
    }

    public DynamoDbQueryBuilder<T> ByGsi(Expression<Func<T, string>> property, string value, DynamoDbOperator queryOperator = DynamoDbOperator.Equal) 
        => ByGsi(GetGsiName(property), GetPropertyName(property), value, queryOperator);

    public DynamoDbQueryBuilder<T> ByGsi(
        string gsiName, 
        string propertyName,
        string value,
        DynamoDbOperator queryOperator = DynamoDbOperator.Equal)
    {
        GsiPropertyName = gsiName;
        _conditions.Add(DynamoDbCondition.Create(propertyName, queryOperator, value));
        return this;
    }

    public async Task<T> FindAsync()
    {
        var result = await QueryAsync();
        return result.Data.SingleOrDefault();
    }

    public async Task<Pagination<T>> QueryAsync()
    {
        if (!string.IsNullOrEmpty(_multiTenantUserId))
            _conditions.Add(DynamoDbCondition.Create(GetGsiName<TenantEntity, string>(x => x.UserId), DynamoDbOperator.Equal, _multiTenantUserId));
        
        var queryOperationConfig = new QueryOperationConfig
        {
            Select = SelectValues.SpecificAttributes,
            IndexName = GsiName,
            Limit = _pageSize,
            Filter = _conditions.ToQueryFilter(),
            AttributesToGet = GetAllProperties(typeof(T))
        };
        var table = _dynamoDbContext.GetTargetTable<T>();
        var search = table.Query(queryOperationConfig);
        var documents = await search.GetNextSetAsync();
        var entities = _dynamoDbContext.ToEntities<T>(documents);
        
        return new Pagination<T>
        {
            Data = entities
        };
    }

    public async Task<Pagination<T>> ScanAsync()
    {
        if (!string.IsNullOrEmpty(_multiTenantUserId))
            _conditions.Add(DynamoDbCondition.Create(GetGsiName<TenantEntity, string>(x => x.UserId), DynamoDbOperator.Equal, _multiTenantUserId));
        
        var scanOperationConfig = new ScanOperationConfig
        {
            Select = SelectValues.SpecificAttributes,
            IndexName = GsiName,
            Limit = _pageSize,
            Filter = _conditions.ToScanFilter(),
            AttributesToGet = GetAllProperties(typeof(T))
        };
        var table = _dynamoDbContext.GetTargetTable<T>();
        var search = table.Scan(scanOperationConfig);
        var documents = await search.GetNextSetAsync();
        var entities = _dynamoDbContext.ToEntities<T>(documents);
        
        return new Pagination<T>
        {
            Data = entities
        };
    }

    private static string GetGsiName<T,P>(Expression<Func<T, P>> action) where T : class
    {
        var expression = (MemberExpression)action.Body;
        
        var dynamoDbGsi = (DynamoDbGsi?) expression.Member.GetCustomAttributes(true).FirstOrDefault(x => typeof(DynamoDbGsi).IsAssignableFrom(x.GetType()));
        if (dynamoDbGsi is not null)
            return dynamoDbGsi.GsiName;
        
        return expression.Member.Name;
    }

    private static string GetPropertyName<T,P>(Expression<Func<T, P>> action) where T : class
    {
        var expression = (MemberExpression)action.Body;
        
        var dynamoDbPropertyAttribute = (DynamoDBPropertyAttribute?) expression.Member.GetCustomAttributes(true).FirstOrDefault(x => typeof(DynamoDBPropertyAttribute).IsAssignableFrom(x.GetType()));
        if (dynamoDbPropertyAttribute is not null)
            return dynamoDbPropertyAttribute.AttributeName;
        
        return expression.Member.Name;
    }

    private static List<string> GetAllProperties(Type type)
    {
        var propertiesName = new Dictionary<string, string>();
        var properties = type.GetProperties();
        
        foreach (var propertyInfo in properties)
        {
            var isInner = propertyInfo.GetCustomAttribute<DynamoDbInner>();
            if (isInner is null)
            {
                var columnName = propertyInfo.GetCollumnName();
                propertiesName.TryAdd(columnName, columnName);
            }
            else
            {
                var listGenericInterface = propertyInfo.PropertyType.GetInterfaces()[0];
                var entityTypeGeneric = listGenericInterface.GetGenericArguments()[0];
                foreach (var property in GetAllProperties(entityTypeGeneric))
                    propertiesName.TryAdd(property, property);
            }
        }

        return propertiesName.Select(x => x.Key).ToList();
    }
}