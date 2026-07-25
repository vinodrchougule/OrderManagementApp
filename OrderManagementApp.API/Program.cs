using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OrderManagementApp.API.Middleware;
using OrderManagementApp.API.Services;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.BLL.Services;
using OrderManagementApp.Common.Data;
using OrderManagementApp.Common.Settings;
using OrderManagementApp.DAL;
using OrderManagementApp.DAL.Interceptors;
using OrderManagementApp.DAL.Repositories;
using OrderManagementApp.Domain.Interfaces;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Declare Connection String
var TestDbconnectionString = builder.Configuration.GetConnectionString("TestDbConnectionString");

var sinkOptions = new MSSqlServerSinkOptions
{
    TableName = "Logs",
    SchemaName = "dbo",
    AutoCreateSqlTable = false,
};


var columnOptions = new ColumnOptions();
columnOptions.Store.Remove(StandardColumn.Properties);
columnOptions.Store.Add(StandardColumn.Properties);
columnOptions.TimeStamp.DataType = System.Data.SqlDbType.DateTime;
columnOptions.TimeStamp.ConvertToUtc = true;

Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                            .MinimumLevel.Override("System", LogEventLevel.Warning)
                            .Enrich.FromLogContext()
                            .WriteTo.Console()
                            .WriteTo.MSSqlServer(
                                        connectionString: TestDbconnectionString,
                                        sinkOptions: sinkOptions,
                                        columnOptions: columnOptions)
                             .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditInterceptor>();

//Add Connection String
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(TestDbconnectionString));

//JWT Settings
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

//Authentication
builder.Services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey!)),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            return Task.CompletedTask;
                        }
                    };
                });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddSingleton<IDbConnectionFactory>(sp => new SqlConnectionFactory(TestDbconnectionString!));

//DAL
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();

//BLL
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

SqlMapper.AddTypeHandler(new OrderStatusTypeHandler());

//Controller + JSON Options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Order Management API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token.\n\nExample: \"Bearer eyJhbGci...\""
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed: 0.0000}";
});

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGrpcReflectionService();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<GrpcOrderService>();

try
{
    Log.Information("OrderManagementApp starting up..");
    app.Run();
}
catch(Exception ex)
{
    Log.Fatal(ex, "OrderManagementApp failed to start.");
}
finally
{
    Log.CloseAndFlush();
}


