using Api.Mappings;
using Api.Services;
using Api.Services.Implementation;
using Database;
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
            // connect to sqlite database (db in local file LocalDatabase.db)            
            options.UseSqlite(builder.Configuration.GetConnectionString("WebApiDatabase"),
                optionsBuilder => optionsBuilder.MigrationsAssembly("Api")) //migrations need to be run from root project/assembly
                .EnableSensitiveDataLogging(); // probably would remove this EF core logging, instead logging custom event (key-value pair) data
        });

        builder.Services.AddScoped<IClassDataAccessService, ClassDataAccessService>();

        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

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

