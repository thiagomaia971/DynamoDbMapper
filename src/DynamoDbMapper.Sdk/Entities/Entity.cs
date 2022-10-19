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
    public string Id { get; private set; }

    [DynamoDBRangeKey("Hash")]
    [JsonProperty("Hash")]
    public string Hash { get; private set; }
    
    [DynamoDbGsi("GSI-EntityType")]
    [DynamoDBProperty("EntityType")]
    [JsonProperty("EntityType")]
    public string EntityType { get; private set; }
    
    [DynamoDbGsi("GSI-InheredType")]
    [DynamoDBProperty("InheredType")]
    [JsonProperty("InheredType")]
    public string InheredType { get; private set; }
    
    [DynamoDbGsi("GSI1-Id")]
    [DynamoDBProperty("GSI1-Id")]
    [JsonProperty("GSI1-Id")]
    public string Gsi1Id { get; set; }
    
    [DynamoDbGsi("GSI1-Hash")]
    [DynamoDBProperty("GSI1-Hash")]
    [JsonProperty("GSI1-Hash")]
    public string Gsi1Hash { get; set; }

    public Entity()
    {
        EntityType = GetType().Name;
        InheredType = EntityType;
        Id = Guid.NewGuid().ToString();
        Hash = Id;
    }
}