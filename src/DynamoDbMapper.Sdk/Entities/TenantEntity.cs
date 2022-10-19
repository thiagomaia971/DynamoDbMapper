using Amazon.DynamoDBv2.DataModel;

namespace DynamoDbMapper.Sdk.Entities;

public class TenantEntity : Entity
{
    [DynamoDBProperty("UserId")]
    public string UserId { get; set; }
}