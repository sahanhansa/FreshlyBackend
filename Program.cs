using FreshlyBackendNew.Data;
using FreshlyBackendNew.Services.Implementations;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Amazon.S3;
using FreshlyBackendNew.Services;
using Amazon;
using Microsoft.IdentityModel.Tokens;
using System;
<<<<<<< HEAD
using System.Text.Json;
=======
using System.Text;
>>>>>>> c8f2f92e632fd0c2938450bb158be0b45019e2f9
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure EF Core with MySQL
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("MySQLConnection"),
//        new MySqlServerVersion(new Version(8, 0, 21))
//    )
//);

// Configure EF Core with Azure
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("AzureMySqlConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("AzureMySqlConnection"))
    ));


<<<<<<< HEAD
// Add CORS services with comprehensive settings
=======
// Add CORS services
>>>>>>> c8f2f92e632fd0c2938450bb158be0b45019e2f9
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
<<<<<<< HEAD
            policy.WithOrigins("http://localhost:4200", "http://localhost:4000", "http://127.0.0.1:4200") // Common Angular app URLs
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// Register application services
// Use fully qualified names to avoid any ambiguity
builder.Services.AddScoped<FreshlyBackendNew.Services.IAllPickupService, FreshlyBackendNew.Services.AllPickupService>();
builder.Services.AddScoped<FreshlyBackendNew.Services.IAllDeliveryService, FreshlyBackendNew.Services.AllDeliveryService>();
builder.Services.AddScoped<ILaundryService, LaundryService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ITemporaryOrderService, TemporaryOrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Add controller services with improved JSON handling
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle null values properly
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        // Handle circular references
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS middleware
app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
=======
            policy.WithOrigins("http://localhost:4200") //  Angular app's URL
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Configure AWS S3 - More explicit configuration
        builder.Services.AddSingleton<IAmazonS3>(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.EUNorth1,
                UseHttp = false,
                UseAccelerateEndpoint = false
            };
    
            var credentials = new Amazon.Runtime.BasicAWSCredentials(
                configuration["AWS:AccessKey"], 
                configuration["AWS:SecretKey"]
            );
    
            return new AmazonS3Client(credentials, config);
        });

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
    };
});

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") //  Angular app's URL
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


// Register all application services in one place
// Use fully qualified name to avoid ambiguity
builder.Services.AddScoped<FreshlyBackendNew.Services.Interfaces.IOrderService, FreshlyBackendNew.Services.Implementations.OrderService>();
builder.Services.AddScoped<ILaundryService, LaundryService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITemporaryOrderService, TemporaryOrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAllPickupService, AllPickupService>();
builder.Services.AddScoped<IAllDeliveryService, AllDeliveryService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IFileStorageService, S3StorageService>();
builder.Services.AddScoped<IServiceService, ServicesService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();



// Add controller services with improved JSON handling
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle null values properly
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        // Handle circular references
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Add detailed error pages in development
    app.UseDeveloperExceptionPage();
    
    // Comment out HTTPS redirection in development for testing
    // app.UseHttpsRedirection();
}
else
{
    app.UseHttpsRedirection();
}

// Use CORS middleware
app.UseCors("AllowAngularApp");

// Add authentication middleware before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
>>>>>>> c8f2f92e632fd0c2938450bb158be0b45019e2f9
