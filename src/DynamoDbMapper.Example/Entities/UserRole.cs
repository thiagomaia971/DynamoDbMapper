using Amazon.DynamoDBv2.DataModel;
using DynamoDbMapper.Sdk.Entities;

namespace DynamoDbMapper.Example.Entities;

[DynamoDBTable("DynamoDbMapperExample")]
public class UserRole : Entity
{
    [DynamoDBProperty("RoleId")]
    public string RoleId { get; set; }
}
