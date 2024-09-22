using System.Reflection;
using VerticalSliceTemplate.Api;
using VerticalSliceTemplate.Api.Shared.Messaging;
using VerticalSliceTemplate.Api.Shared.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        var schemaHelper = new SwashbuckleSchemaHelper();
        options.CustomSchemaIds(type => schemaHelper.GetSchemaId(type));
    });

builder.Services
    .RegisterApplicationServices()
    .RegisterPersistence(builder.Configuration)
    .AddEndpoints(Assembly.GetExecutingAssembly())
    .RegisterMessaging(Assembly.GetExecutingAssembly());

builder.Services
    .AddHttpContextAccessor();

var webApplication = builder.Build();

if (webApplication.Environment.IsDevelopment())
{
    webApplication
        .UseSwagger()
        .UseSwaggerUI();
}

webApplication.MapEndpoints();

await webApplication.Services.InitializeDatabase();

await webApplication.RunAsync();