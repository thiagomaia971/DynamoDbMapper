using DynamoDbMapper.Example.Entities;
using DynamoDbMapper.Example.Repositories;
using DynamoDbMapper.Sdk.Configurations;
using DynamoDbMapper.Sdk.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRepositories(typeof(Entity));
builder.Services.AddDynamodbMapper(builder.Configuration, builder.Environment);
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "";
    });
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/user", async ([FromServices] IUserRepository userRepository) =>
    {
        return await userRepository.GetAll();
    })
    .WithTags("User")
    .WithOpenApi();

app.MapGet("/user/{id}", async ([FromRoute] string id, [FromServices] IUserRepository userRepository) =>
    {
        return await userRepository.FindById(id);
    })
    .WithTags("User")
    .WithOpenApi();

app.MapPost("/user", async ([FromBody] User user, [FromServices] IUserRepository userRepository) =>
    {
        if (!string.IsNullOrEmpty(user.PrimaryKey))
        {
            var entityExist =  await userRepository
                .CreateQuery()
                .ByGsi(x => x.PrimaryKey, user.PrimaryKey)
                .ByInheritedType()
                .FindAsync();
            
            if (entityExist is not null)
                return Results.Created(string.Empty, entityExist);
        }
                
        var outputDto = await userRepository.Save(user);
        return Results.Ok(outputDto);
    })
    .WithTags("User")
    .WithOpenApi();

app.MapPut("/user/{id}", async ([FromRoute] string id, [FromBody] User user, [FromServices] IUserRepository userRepository) =>
    {
        var entity = await userRepository.CreateQuery().ById(id).FindAsync();
        var oldEntity = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(entity));

        if (entity is null)
            return Results.NotFound();

        //var oldForeingKey = entity.ForeingKey;
        // var entityMapped = mapper.Map(request.Payload, entity);
        //var newForeingKey = entity.ForeingKey;
        // if (user.ForeingKey != oldEntity.ForeingKey) await userRepository.Remove(oldEntity);
        return Results.Ok(await userRepository.Save(user));
    })
    .WithTags("User")
    .WithOpenApi();

app.MapDelete("/user/{id}", async ([FromRoute] string id, [FromServices] IUserRepository userRepository) =>
    {
        var entity = await userRepository.FindById(id);
            
        if (entity is null)
            return Results.NotFound();
        await userRepository.Remove(entity);
        return Results.Ok(entity);
    })
    .WithTags("User")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}