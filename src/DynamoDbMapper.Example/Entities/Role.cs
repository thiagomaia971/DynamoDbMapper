using Amazon.DynamoDBv2.DataModel;
using DynamoDbMapper.Sdk.Entities;

namespace DynamoDbMapper.Example.Entities;

[DynamoDBTable("DynamoDbMapperExample")]
public class Role : Entity
{
    [DynamoDBProperty("Name")]
    public string Name { get; set; }
}