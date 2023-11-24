using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DynamoDbMapper.Example.Entities;
using DynamoDbMapper.Sdk.Entities;
using DynamoDbMapper.Sdk.Interfaces;
using DynamoDbMapper.Sdk.Repositories;

namespace DynamoDbMapper.Example.Repositories;

public interface IUserRepository : IRepository<User>;

public class UserRepository
    (
        IDynamoDBContext dynamoDbContext, 
        IAmazonDynamoDB amazonDynamoDb,
        MultiTenantScoped multiTenant
    )
    : Repository<User>(dynamoDbContext, amazonDynamoDb, multiTenant), IUserRepository;