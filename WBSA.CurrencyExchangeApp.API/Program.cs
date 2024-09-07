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
    options.InstanceName = "CurrencyExchange_";
});
builder.Services.AddDistributedMemoryCache(); // Required for session state
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(20); // Set session timeout
    options.Cookie.HttpOnly = true; // Protect session cookie
    options.Cookie.IsEssential = true; // Make the cookie essential
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
// Enable session middleware
app.UseSession();
// Add custom middleware to manage session timeout
app.Use(async (context, next) =>
{
    // Fixed session timeout duration (e.g., 60 minutes)
    TimeSpan sessionMaxDuration = TimeSpan.FromMinutes(30);

    // Retrieve session start time
    string sessionStartTimeStr = context.Session.GetString("SessionStartTime");

    if (string.IsNullOrEmpty(sessionStartTimeStr))
    {
        // If no start time is found, set the session start time
        context.Session.SetString("SessionStartTime", DateTime.UtcNow.ToString());
    }
    else
    {
        // Parse the stored session start time
        DateTime sessionStartTime = DateTime.Parse(sessionStartTimeStr);

        // Check if the session has exceeded the allowed duration
        if (DateTime.UtcNow - sessionStartTime > sessionMaxDuration)
        {
            // Clear session if the session duration exceeds the max limit
            //context.Session.Clear();
            return;
        }
    }

    await next.Invoke();
});
app.UseAuthentication();
app.UseAuthorization();


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