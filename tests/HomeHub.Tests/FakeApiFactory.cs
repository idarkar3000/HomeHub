using HomeHub.Repositories;
using Microsoft.AspNetCore.Authentication;
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
        builder.UseSetting("Jwt:Key", "test-key-for-homehub-tests-1234567890");
        builder.UseSetting("Jwt:Issuer", "HomeHub");
        builder.UseSetting("Jwt:Audience", "HomeHub");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IRoomRepository>();
            services.RemoveAll<ITaskRepository>();
            services.RemoveAll<INoteRepository>();
            services.RemoveAll<IUserRepository>();

            services.AddSingleton<IRoomRepository, FakeRoomRepository>();
            services.AddSingleton<ITaskRepository, FakeTaskRepository>();
            services.AddSingleton<INoteRepository, FakeNoteRepository>();
            services.AddSingleton<IUserRepository, FakeUserRepository>();

            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, null);
        });
    }
}
