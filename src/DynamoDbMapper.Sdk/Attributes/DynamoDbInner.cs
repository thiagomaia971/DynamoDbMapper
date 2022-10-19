namespace DynamoDbMapper.Sdk.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class DynamoDbInner : Attribute
{
    public Type Type { get; }

    public DynamoDbInner(Type type) 
        => Type = type;
}