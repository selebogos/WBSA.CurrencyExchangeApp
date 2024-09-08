using Mapster;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using WBSA.CurrencyExchangeApp.API.Middlewares;
using WBSA.CurrencyExchangeApp.Data.Extensions;
using WBSA.CurrencyExchangeApp.Services.Extensions;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();


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

//var redisConnection=Environment.GetEnvironmentVariable("RedisConnection");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "CurrencyExchange_";
});
builder.Services.AddDistributedMemoryCache(); // Required for session state
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Set session timeout
    options.Cookie.HttpOnly = true; // Protect session cookie
    options.Cookie.IsEssential = true; // Make the cookie essential
});
// Add services to the container.
builder.Services.AddCurrencyExchangeServices();
builder.Services.AddCurrencyExchangeSettings(builder.Configuration);
builder.Services.AddDataServices(builder.Configuration);


builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "World Sport Betting Currency Exchange Solution",
            Version = "v1",
            Description = "This is a Currency Exchange API.",
        });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


var app = builder.Build();

app.UseStaticFiles();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var env = app.Environment;
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        var sidebar = Path.Combine(env.ContentRootPath, "wwwroot/custom-sidebar.html");
        c.HeadContent = File.ReadAllText(sidebar);
        c.InjectStylesheet("/swagger-custom.css");
        c.InjectJavascript("/swagger-custom.js");

        c.InjectJavascript("https://code.jquery.com/jquery-3.6.0.min.js");
    });
    
    
}

app.UseHttpsRedirection();

app.UseRouting();
// Enable session middleware
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();


// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();
app.InitializeDatabase();

app.Run();