using Api.Mappings;
using Api.Services;
using Api.Services.Implementation;
using Api.Validation;
using Database;
using Domain.Model;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<ApiContext>(options =>
        {
            // Configure SQL Server connection string
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerDatabase"), options =>
            {
                options.MigrationsAssembly("Api");
                options.EnableRetryOnFailure();
            })
            .EnableSensitiveDataLogging();            
        });

        builder.Services.AddScoped<IClassDataAccessService, ClassDataAccessService>();
        builder.Services.AddScoped<IStudentDataAccessService, StudentDataAccessService>();
        builder.Services.AddScoped<ITeacherDataAccessService, TeacherDataAccessService>();

        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        builder.Services.AddScoped<IValidator<(Class cls, Student student)>, StudentEnrollmentValidator>();
        builder.Services.AddScoped<IValidator<(Teacher, Class)>, AssignClassValidator>();

        builder.Services.AddHttpLogging(o => { });

        var app = builder.Build();

        // Apply migrations at startup
        MigrateDB(app);

        // Configure the HTTP request pipeline.
        app.UseHttpLogging();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void MigrateDB(WebApplication app)
    {        
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApiContext>();
                context.Database.Migrate(); // Apply migrations
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
            }
        }
    }
}

