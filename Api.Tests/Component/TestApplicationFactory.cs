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
    protected override void ConfigureClient(HttpClient client)
    {
        // use to config http client before setting off requests
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // use to mock out some services for testing
        builder.ConfigureTestServices(services =>
        {         
            services.AddDbContext<ApiContext>(options =>
               options.UseSqlite("Filename=:memory:")); // use in memory sqllite database
        });        
    }
}
