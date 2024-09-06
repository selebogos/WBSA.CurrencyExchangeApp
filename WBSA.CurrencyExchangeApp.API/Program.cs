using Mapster;
using System.Text.Json.Serialization;
using WBSA.CurrencyExchangeApp.API.Middlewares;
using WBSA.CurrencyExchangeApp.Data.Extensions;
using WBSA.CurrencyExchangeApp.Services.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses 
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    // ignore omitted parameters on models to enable optional params 
    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddMapster();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    //options.InstanceName = "CurrencyExchange_";
});
builder.Services.AddCurrencyExchangeServices();
builder.Services.AddCurrencyExchangeSettings(builder.Configuration);
builder.Services.AddDataServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
//app.UseAuthorization();


// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();
//app.UseMiddleware<AuthenticationMiddleware>();

app.MapControllers();
app.InitializeDatabase();

app.Run();