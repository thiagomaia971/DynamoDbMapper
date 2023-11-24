using System.Diagnostics.CodeAnalysis;
using Amazon.DynamoDBv2.DataModel;
using DynamoDbMapper.Sdk.Attributes;
using Newtonsoft.Json;

namespace DynamoDbMapper.Sdk.Entities;

[ExcludeFromCodeCoverage]
public abstract class Entity
{
    [DynamoDBHashKey("Id")] 
    [JsonProperty("Id")]
    public string Id { get; set; }

    [DynamoDBRangeKey("CreatedAt")]
    [DynamoDbGsi("GSI-CreatedAt")]
    [JsonProperty("CreatedAt")]
    public string CreatedAt { get; set; }

    [DynamoDbGsi("GSI-ForeingKey")]
    [JsonProperty("ForeingKey")]
    public string ForeingKey { get; set; }
    
    [DynamoDbGsi("GSI-EntityType")]
    [DynamoDBProperty("EntityType")]
    [JsonProperty("EntityType")]
    public string EntityType { get; protected set; }
    
    [DynamoDbGsi("GSI-InheritedType")]
    [DynamoDBProperty("InheritedType")]
    [JsonProperty("InheritedType")]
    public string InheritedType { get; set; }
    
    [DynamoDbGsi("GSI-PrimaryKey")]
    [DynamoDBProperty("GSI-PrimaryKey")]
    [JsonProperty("GSI-PrimaryKey")]
    public string PrimaryKey { get; set; }
    
    [DynamoDbGsi("GSI-PrimaryForeingKey")]
    [DynamoDBProperty("GSI-PrimaryForeingKey")]
    [JsonProperty("GSI-PrimaryForeingKey")]
    public string PrimaryForeingKey { get; set; }
    
    [DynamoDBProperty("UpdatedAt")]
    [JsonProperty("UpdatedAt")]
    public string UpdatedAt { get; set; }

    public Entity()
    {
        EntityType = GetType().Name;
        Id = Guid.NewGuid().ToString();
        // ForeingKey = Id;
        CreatedAt = DateTime.Now.ToString("O");
    }
}
