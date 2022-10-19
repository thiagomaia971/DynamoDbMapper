namespace DynamoDbMapper.Sdk.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class DynamoDbGsi : Attribute
{
    public string GsiName { get; }

    public DynamoDbGsi(string gsiName) 
        => GsiName = gsiName;
}