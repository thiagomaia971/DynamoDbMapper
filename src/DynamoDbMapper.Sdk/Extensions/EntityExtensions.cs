using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using DynamoDbMapper.Sdk.Entities;

namespace DynamoDbMapper.Sdk.Extensions;

public static class EntityExtensions
{
    public static Dictionary<string, AttributeValue> AttributeValues(this Entity entityInner)
    {
        var dic = new Dictionary<string, AttributeValue>();
        var properties = entityInner.GetPropertiesWithoutAttribute<DynamoDBIgnoreAttribute>();

        foreach (var property in properties)
        {
            var value = property.GetValue(entityInner);
            if (value is null)
                continue;
            
            if (value is System.DateTime dateTime)
                dic.Add(property.GetCollumnName(), new AttributeValue(dateTime.ToString("O")));
            else if (value is System.DateTimeOffset dateTimeOffset)
                dic.Add(property.GetCollumnName(), new AttributeValue(dateTimeOffset.ToString("O")));
            else
                dic.Add(property.GetCollumnName(), new AttributeValue(value.ToString()));
        }

        return dic;
    }
}