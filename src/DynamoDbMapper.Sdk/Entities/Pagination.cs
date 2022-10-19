using Amazon.DynamoDBv2.Model;

namespace DynamoDbMapper.Sdk.Entities;

public class Pagination<T>
{
    public Dictionary<string, AttributeValue> LastEvaluatedKey { get; set; }
    public IEnumerable<T> Data { get; set; }
    public string ResultType { get; set; }
}