using System.Text;
using HomeHub.Endpoints;
using HomeHub.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddTransient<IRoomRepository>(provider => new PostgresRoomRepository(connectionString));
builder.Services.AddTransient<ITaskRepository>(provider => new PostgresTaskRepository(connectionString));
builder.Services.AddTransient<INoteRepository>(provider => new PostgresNoteRepository(connectionString));
builder.Services.AddTransient<IShoppingCategoryRepository>(provider => new PostgresShoppingCategoryRepository(connectionString));
builder.Services.AddTransient<IUserRepository>(provider => new PostgresUserRepository(connectionString));

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt 'Key' not found.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roomRepository = scope.ServiceProvider.GetRequiredService<IRoomRepository>();
    var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
    var noteRepository = scope.ServiceProvider.GetRequiredService<INoteRepository>();
    var categoryRepository = scope.ServiceProvider.GetRequiredService<IShoppingCategoryRepository>();
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    userRepository.InitializeDatabase();
    categoryRepository.InitializeDatabase();
    roomRepository.InitializeDatabase();
    taskRepository.InitializeDatabase();
    noteRepository.InitializeDatabase();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    context.Response.StatusCode = error is UnauthorizedAccessException
        ? StatusCodes.Status401Unauthorized
        : StatusCodes.Status500InternalServerError;

    await context.Response.WriteAsJsonAsync(new { error = "Se ha producido un error al procesar la solicitud." });
}));

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeadersOptions.KnownIPNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthEndpoints();
app.MapRoomEndpoints();
app.MapTaskEndpoints();
app.MapNoteEndpoints();
app.MapShoppingCategoryEndpoints();
app.MapAuthEndpoints();

app.Run();

public partial class Program { }
