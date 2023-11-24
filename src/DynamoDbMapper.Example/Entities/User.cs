using Amazon.DynamoDBv2.DataModel;
using DynamoDbMapper.Sdk.Attributes;
using DynamoDbMapper.Sdk.Entities;

namespace DynamoDbMapper.Example.Entities;

[DynamoDBTable("DynamoDbMapperExample")]
public class User : Entity
{
    [DynamoDBProperty("Name")]
    public string Name { get; set; }

    [DynamoDBProperty("EmailConfirmed")]
    public bool EmailConfirmed { get; set; }

    [DynamoDBProperty("PasswordForeingKey")]
    public string PasswordForeingKey { get; set; }

    [DynamoDBProperty("PhoneNumber")]
    public string PhoneNumber { get; set; }

    [DynamoDBProperty("PhoneNumberConfirmed")]
    public bool PhoneNumberConfirmed { get; set; }

    [DynamoDBProperty("TwoFactorEnabled")]
    public bool TwoFactorEnabled { get; set; }

    [DynamoDBIgnore]
    [DynamoDbInner(typeof(UserRole))]
    public List<UserRole> Roles { get; set; }
}