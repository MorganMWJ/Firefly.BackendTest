using Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Api.Tests.Component;

public class TestApplicationFactory : WebApplicationFactory<Program>
{

    public ApiContext InMemoryContext { get; set; }

    protected override void ConfigureClient(HttpClient client)
    {
        // use to config http client before setting off requests
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // use to mock out some services for testing
            var apiContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ApiContext));
            services.Remove(apiContextDescriptor);

            services.AddDbContext<ApiContext>(options =>
               options.UseSqlite("Filename=:memory:"));// options.UseInMemoryDatabase("TestDb"));

            // Save for access
            InMemoryContext = services.BuildServiceProvider().GetService<ApiContext>();
        });
        
    }
}
