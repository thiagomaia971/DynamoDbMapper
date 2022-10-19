# How to Use

## Create Entity

```C#

[DynamoDBTable("YourTable")]
public class User : Entity
{
    [DynamoDBProperty("Name")]
    public string Name { get; set; }
}
```

## Configure
```c#
services
    .AddDynamodbMapper(configuration, environment)
    .AddRepositories(typeof(User))
```

## Repository

### GetAll

Return all Users and yours SubEntities.

```c#
var users = _repository.GetAll();
```

ou

```c#
var users = _repository
                .CreateQuery()
                .ByGsi("GSI-EntityType", "EntityType", "User", DynamoDbOperator.BeginsWith)
                .ScanAsync();
```

### FindById

Return User and yours SubEntities.

```c#
var users = _repository.FindById("123456");
```

ou

```c#
var users = _repository
                .CreateQuery()
                .ById("123456")
                .ByEntityType(DynamoDbOperator.BeginsWith)
                .QueryAsync();
```


