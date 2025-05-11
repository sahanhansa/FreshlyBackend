using FreshlyBackendNew.Data;
using FreshlyBackendNew.Services;
using FreshlyBackendNew.Services.Implementations;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);// Creates a builder for configuring the web application.

//Sahan-added service layer for customer
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Add DbContext for MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySQLConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    )
);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers();
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

app.UseCors("AllowAngularApp"); // Apply CORS policy

app.UseAuthorization();

app.MapControllers();

app.Run();