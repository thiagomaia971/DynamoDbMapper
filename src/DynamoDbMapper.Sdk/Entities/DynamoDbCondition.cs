namespace DynamoDbMapper.Sdk.Entities;

public class DynamoDbCondition
{
    public string PropertyName { get; }
    public DynamoDbOperator Operator { get; }
    public string Value { get; }

    private DynamoDbCondition(string propertyName, DynamoDbOperator @operator, string value)
    {
        PropertyName = propertyName;
        Operator = @operator;
        Value = value;
    }

    public static DynamoDbCondition Create(string propertyName, DynamoDbOperator @operator, string value)
        => new(propertyName, @operator, value);
}