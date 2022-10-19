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
var users = await _repository.GetAll();
```

ou

```c#
var users = await _repository
                .CreateQuery()
                .ByGsi("GSI-InhiredType", "InhiredType", "User")
                .QueryAsync();
```

### FindById

Return User and yours SubEntities.

```c#
var users = await _repository.FindById("123456");
```

ou

```c#
var users = await _repository
                .CreateQuery()
                .ById("123456")
                .ByEntityType(DynamoDbOperator.BeginsWith)
                .QueryAsync();
```


