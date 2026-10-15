using HomeHub.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HomeHub.Tests;

public class FakeApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(
            "ConnectionStrings:DefaultConnection",
            "Host=localhost;Database=fake;Username=fake;Password=fake");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IRoomRepository>();
            services.RemoveAll<ITaskRepository>();
            services.RemoveAll<INoteRepository>();

            services.AddSingleton<IRoomRepository, FakeRoomRepository>();
            services.AddSingleton<ITaskRepository, FakeTaskRepository>();
            services.AddSingleton<INoteRepository, FakeNoteRepository>();
        });
    }
}
