using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using DynamoDbMapper.Sdk.Entities;
using DynamoDbMapper.Sdk.Interfaces;
using DynamoDbMapper.Sdk.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DynamoDbMapper.Sdk.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDynamodbMapper(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddScoped<MultiTenantScoped>();
        
        if (environment.IsDevelopment())
            return LocalhostConfig(services, configuration);
        
        return services
            .AddDefaultAWSOptions(configuration.GetAWSOptions())
            .AddAWSService<IAmazonDynamoDB>()
            .AddTransient<IDynamoDBContext, DynamoDBContext>();
    }
    
    public static IServiceCollection AddRepositories(
        this IServiceCollection services,
        params Type[] entityScanMarkers)
    {
        foreach (var marker in entityScanMarkers)
        {
            var entityTypes = marker.Assembly.ExportedTypes
                .Where(x => typeof(Entity).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
            
            foreach (var entityType in entityTypes)
            {
                var @interface = typeof(IRepository<>).MakeGenericType(entityType);
                var implementation = typeof(Repository<>).MakeGenericType(entityType);
                services.AddScoped(@interface, implementation);
            }
        }
        
        return services;
    }

    private static IServiceCollection LocalhostConfig(IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDefaultAWSOptions(configuration.GetAWSOptions())
            .AddSingleton<IAmazonDynamoDB>(c => new AmazonDynamoDBClient(
                new BasicAWSCredentials("fake", "fake"),
                new AmazonDynamoDBConfig
                {
                    UseHttp = true,
                    ServiceURL = configuration["AWS:Dynamodb:ServiceUrl"]
                }))
            .AddSingleton<IDynamoDBContext>(provider =>
            {
                return new DynamoDBContext(
                    new AmazonDynamoDBClient(new BasicAWSCredentials("fake", "fake"),
                        new AmazonDynamoDBConfig
                        {
                            UseHttp = true,
                            ServiceURL = configuration["AWS:Dynamodb:ServiceUrl"]
                        }));
            });
    }
}