using FreshlyBackendNew.Data;
using FreshlyBackendNew.Services.Implementations;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Amazon.S3;
using FreshlyBackendNew.Services;
using Amazon;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure EF Core with MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySQLConnection"),
        new MySqlServerVersion(new Version(8, 0, 21)) // Specify the MySQL server version here
    )
);
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


// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "http://localhost:5027") //  Angular app's URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Register application services
builder.Services.AddScoped<ILaundryService, LaundryService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ITemporaryOrderService, TemporaryOrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAllPickupService, AllPickupService>();
builder.Services.AddScoped<IAllDeliveryService, AllDeliveryService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IFileStorageService, S3StorageService>();
builder.Services.AddScoped<IServiceService, ServicesService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();




// Add controller services
builder.Services.AddControllers();

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
