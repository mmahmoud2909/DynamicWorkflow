using DynamicWorkflow.APIs.Extenstions;
using DynamicWorkflow.Infrastructure.DataSeeding;
using DynamicWorkflow.Infrastructure.Identity;
using DynamicWorkflow.Services.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

namespace DynamicWorkflow.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthServices(builder.Configuration);
            builder.Services.AddScoped<WorkflowInstanceService>();
            builder.Services.AddApplicationsService(builder.Configuration);
            builder.Services.AddControllers()
    .      AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

            builder.Services.AddEndpointsApiExplorer();
          
            //// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dynamic Workflow API", Version = "v1" });
                c.UseInlineDefinitionsForEnums();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            var app = builder.Build();

            //Data Seeding Scope
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // ?? 1?? Seed users and roles
                await UsersAndRolesSeedData.SeedAsync(services);

                // ?? 2?? Seed base workflows (LV Plant etc.)
                await ApplicationdbcontextSeed.SeedDataAsync(services);

                // ?? 3?? Optional: seed ServiceRepairProcurement workflows
                var context = services.GetRequiredService<ApplicationIdentityDbContext>();
                var serviceRepairWorkflows = ServiceRepairWorkflowSeedData.GetWorkflows();
                context.Workflows.AddRange(serviceRepairWorkflows);
                await context.SaveChangesAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dynamic Workflow API V1");
                    options.RoutePrefix = string.Empty; // Swagger UI at root URL (https://localhost:7180)
                });
            }

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

                    if (exceptionHandlerPathFeature?.Error != null)
                    {
                        var logger = app.Services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(exceptionHandlerPathFeature.Error, "Unhandled exception occurred.");
                    }
                });
            });

            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            var webRootPath = app.Environment.WebRootPath;
            if (string.IsNullOrEmpty(webRootPath))
            {
                // Fallback to default wwwroot path
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var uploadsPath = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")),
                RequestPath = "/uploads"
            });
          
            //WorkflowSeedData.GetWorkflows();
            app.SeedWorkflowData();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}

