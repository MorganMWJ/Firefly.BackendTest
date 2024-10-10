using Api.Mappings;
using Api.Services;
using Api.Services.Implementation;
using Api.Validation;
using Database;
using Domain.Model;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

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
            // connect to SQL Server DB - run this before via command "docker compose up -d"
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer:WebApiDatabase"),
                b => b.MigrationsAssembly("Api"))
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
}

