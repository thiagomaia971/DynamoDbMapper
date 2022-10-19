using System.Diagnostics;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DynamoDbMapper.Sdk.Attributes;
using DynamoDbMapper.Sdk.Entities;
using Newtonsoft.Json;

namespace DynamoDbMapper.Sdk.Extensions;

public static class DocumentExtensions
{
    public static IEnumerable<T> ToEntities<T>(this IDynamoDBContext dynamoDbContext, List<Document> documents)
        where T : Entity
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        
        var entities = new List<T>();
        var others = new Dictionary<(string id, string entityType), List<string>>();
        foreach (var document in documents)
        {
            var documentEntityType = document["EntityType"].AsString();
            if (documentEntityType == Activator.CreateInstance<T>().EntityType)
                entities.Add(dynamoDbContext.FromDocument<T>(document));
            else
            {
                var json = document.ToJson();
                var documentId = document["Id"].AsString();
                if (others.ContainsKey((documentId, documentEntityType)))
                    others[(documentId, documentEntityType)].Add(json);
                else
                    others.Add((documentId, documentEntityType), new List<string>{ json });
            }
        }

        if (others.Count == 0)
            return entities;

        foreach (var entity in entities)
        {
            var properties = entity.GetType()
                .GetProperties()
                .Where(x =>
                    x.GetCustomAttributes(false)
                        .FirstOrDefault(y => typeof(DynamoDbInner).IsAssignableFrom(y.GetType())) != null);
            
            foreach (var propertyInfo in properties)
            {
                var dynamoDbInner = (DynamoDbInner) propertyInfo.GetCustomAttributes(false)
                    .FirstOrDefault(x => typeof(DynamoDbInner).IsAssignableFrom(x.GetType()));
                var type = dynamoDbInner.Type;
                var list = others[(entity.Id, type.Name)].Select(x => JsonConvert.DeserializeObject(x, type));
                
                var makeGenericType = typeof(List<>).MakeGenericType(type);
                var instance = Activator.CreateInstance(makeGenericType);
                foreach (var o in list)
                {
                    instance.GetType().GetMethod("Add").Invoke(instance, new []{o});
                    propertyInfo.SetValue(entity,   instance);
                }
            }
        }
        stopWatch.Stop();
        Console.WriteLine($"Stops in: {stopWatch.ElapsedMilliseconds}ms");
        return entities;
    }
}