using FreshlyBackendNew.Data;
using FreshlyBackendNew.Services;
using FreshlyBackendNew.Services.Implementations;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure EF Core with MySQL
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("MySQLConnection"),
//        new MySqlServerVersion(new Version(8, 0, 21))
//    )
//);

//Configure EF Core with Azure
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("AzureMySqlConnection"),
       ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("AzureMySqlConnection"))
    ));


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
