using FreshlyBackendNew.Data;
using FreshlyBackendNew.Services;
using FreshlyBackendNew.Services.Implementations;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace FreshlyBackendNew
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);// Creates a builder for configuring the web application.

            // Configure EF Core with MySQL
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("MySQLConnection"),
                    new MySqlServerVersion(new Version(8, 0, 21))
                )
            );

            // Add CORS services with comprehensive settings
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200", "http://localhost:4000", "http://127.0.0.1:4200") // Common Angular app URLs
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
            });

            // Register all application services in one place
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ILaundryService, LaundryService>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITemporaryOrderService, TemporaryOrderService>();

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
        }
    }
}