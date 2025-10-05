using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Interfaces;
using DynamicWorkflow.Core.Interfaces.Repositories;
using DynamicWorkflow.Infrastructure.Data;
using DynamicWorkflow.Infrastructure.Identity;
using DynamicWorkflow.Infrastructure.Repositories;
using DynamicWorkflow.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace DynamicWorkflow.APIs.Extenstions
{
    public static class ApplicationServiceExtenstions
    {
        public static IServiceCollection AddApplicationsService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }); 
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(ILoggingService<>), typeof(LoggingService<>));
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<StepService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IStateMachineFactory, StateMachineFactory>();
            services.AddScoped<IWorkflowService, WorkflowService>();
            services.AddScoped<IWorkflow, WorkflowRepository>();
            services.AddScoped<IAdminUserService, AdminUserService>();
            services.AddScoped<IAdminWorkflowService, AdminWorkflowService>();
            services.AddScoped<IworkflowInstanceService, WorkflowInstanceServices>();


            // This automatically scans for and registers all Profile classes
            // Or register specific profiles
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<Core.Mapping.MappingProfile>();
            });

            services.AddCors();

            services.AddCors();
            return services;
        }
        public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationIdentityDbContext>(options =>
             options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging(), ServiceLifetime.Scoped);
            services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationIdentityDbContext>().AddDefaultTokenProviders();
            //services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            //{
            //    options.Password.RequireDigit = true;
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireUppercase = true;
            //    options.Password.RequireLowercase = false;
            //})
            //        .AddEntityFrameworkStores<ApplicationIdentityDbContext>()

            services.AddHttpClient();
            var tokenSection = configuration.GetSection("Token");//
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = "https://localhost:7180",
                                ValidAudience = configuration["Token:Audience"],
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"]!)),
                                RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                            };
            });

            services.AddAuthorization(options => 
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN","Admin"));
            });
            return services;
        }
    }
}
