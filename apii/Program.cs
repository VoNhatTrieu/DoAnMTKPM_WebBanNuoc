using Microsoft.EntityFrameworkCore;
using apii.Data;
using apii.Repositories;
using apii.Services;

namespace apii
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Add DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add HttpContextAccessor for user context
            builder.Services.AddHttpContextAccessor();
            
            // Add User Context for Ownership Validation
            builder.Services.AddScoped<apii.Models.IUserContext, apii.Models.UserContext>();

            // Add Repository Pattern
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Add Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<SeedDataService>();

            // Add Controllers
            builder.Services.AddControllers();

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Web Bán Nước API - Sip & Savor",
                    Version = "v1",
                    Description = "API cho website bán nước trực tuyến với đầy đủ chức năng đăng ký, đăng nhập, quản lý sản phẩm, giỏ hàng và đơn hàng",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Support Team",
                        Email = "support@sipandsavor.vn"
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            // Enable Swagger in all environments
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web Bán Nước API v1");
                c.RoutePrefix = string.Empty; // Swagger UI at root
            });

            // app.UseHttpsRedirection(); // Tắt HTTPS redirect cho development

            // Use CORS
            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
